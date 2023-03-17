using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class PlayerLocomotion : MonoBehaviour
    {
        Transform cameraObject;
        InputManager inputManager;
        Vector3 moveDirection;
        PlayerManager playerManager;
        PlayerStats playerStats;
        CameraManager cameraManager;

        [HideInInspector]
        public Transform playerTransform;
        [HideInInspector]
        public PlayerAnimatorManager animatorManager;
        [HideInInspector]
        public new Rigidbody rigidbody;
        [HideInInspector]
        public GameObject normalCamera;     //TODO:之后还会有个锁定目标的相机。

        [Header("Movement Stats")]
        [SerializeField]
        float moveSpeed;
        [SerializeField]
        float rotationSpeed;
        [SerializeField]
        float sprintSpeed;
        [SerializeField]
        float walkSpeed;
        [SerializeField]
        float fallSpeed = 60;

        [Header("Ground&Air Check Stats")]
        [SerializeField]
        float groundDetectionRayStartPoint = 0.0f;
        [SerializeField]
        float minDistanceNeededToBeginFall = 1.5f;
        [SerializeField]
        float groundDetctionOffset = 0.2f;
        public float inAirTimer;
        LayerMask groundCheckIgnore;

        [Header("Stima Costs")]
        public int sprintStimaCost;
        public int rollingStimaCost;

        public CapsuleCollider charaCollider;
        public CapsuleCollider combatCollider;

        private void Start()
        {
            Physics.IgnoreCollision(charaCollider, combatCollider, true);
            playerStats = GetComponent<PlayerStats>();
            playerManager = GetComponent<PlayerManager>();
            animatorManager = GetComponentInChildren<PlayerAnimatorManager>();
            animatorManager.Initialize();
            cameraObject = Camera.main.transform;
            inputManager = GetComponent<InputManager>();
            rigidbody = GetComponent<Rigidbody>();
            cameraManager = FindObjectOfType<CameraManager>();
            playerTransform = transform;

            playerManager.isGrounded = true;
            groundCheckIgnore = ~(1 << 8 | 1 << 9 | 1 << 10 | 1 << 11);
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            HandleJump(delta);
            HandleRoll(delta);     //控制翻滚

        }

        /// <summary>
        /// 与RigidBody有关的建议放在FixedUpdate中。
        /// </summary>
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            HandleMovement(delta);          //控制移动
            HandleFall(delta, moveDirection);
            HandleRotation(delta);
        }

        #region Movement

        Vector3 normalVector;
        Vector3 targetPosition;

        /// <summary>
        /// 在跑动时控制角色模型的朝向。
        /// </summary>
        /// <param name="delta"></param>
        void HandleRotation(float delta)
        {
            if (playerManager.isInteracting)
            {
                return;
            }

            if (animatorManager.canRotate == false)
            {
                return;
            }

            //锁定目标时，角色会始终面向目标。
            if (inputManager.lockOnFlag && inputManager.sprintFlag == false)
            {
                //冲刺和翻滚时，仍然会面向指定的方向。
                if (inputManager.sprintFlag || inputManager.rollFlag)
                {
                    Vector3 targetDirection = Vector3.zero;
                    targetDirection = cameraManager.cameraTransform.forward * inputManager.vertical;
                    targetDirection += cameraManager.cameraTransform.right * inputManager.horizontal;
                    targetDirection.Normalize();
                    targetDirection.y = 0;

                    if (targetDirection == Vector3.zero)
                    {
                        targetDirection = cameraManager.cameraTransform.forward;
                    }

                    Quaternion tr = Quaternion.LookRotation(targetDirection);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * 10f);

                    transform.rotation = targetRotation;
                }
                else
                {
                    Vector3 rotationDirection = moveDirection;
                    rotationDirection = cameraManager.currentLockOnTarget.position - transform.position;
                    rotationDirection.Normalize();
                    rotationDirection.y = 0;

                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetDirection = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * 10f);

                    playerTransform.rotation = targetDirection;
                }
            }
            else    //没有锁定目标时
            {
                Vector3 targetDir = Vector3.zero;
                float moveOverride = inputManager.moveAmount;

                targetDir = inputManager.vertical * cameraObject.forward;      //结合xy两个轴上的移动方向。
                targetDir += inputManager.horizontal * cameraObject.right;     //注意这里是根据摄像机的方向，因为是根据移动方向转向，而玩家的移动方向是基于摄像机的方向。

                targetDir.Normalize();
                targetDir.y = 0;

                //没有输入就默认向前跑。
                if (targetDir == Vector3.zero)
                {
                    targetDir = transform.forward;
                }

                float rs = rotationSpeed;

                Quaternion tr = Quaternion.LookRotation(targetDir);     //将旋转目标设置为目标方向。
                Quaternion targetRotation = Quaternion.Slerp(playerTransform.rotation, tr, delta * rs);

                playerTransform.rotation = targetRotation;
            }
        }

        /// <summary>
        /// 控制角色的移动。
        /// </summary>
        /// <param name="delta"></param>
        private void HandleMovement(float delta)
        {

            if (playerManager.isInteracting)
            {
                animatorManager.UpdateAnimatiorValues(0, 0, playerManager.isSprinting, playerManager.isWalking);
                return;
            }
            if (inputManager.rollFlag)
            {
                return;
            }

            moveDirection = cameraObject.forward * inputManager.vertical;   //注意这里是根据摄像机的方向
            moveDirection += cameraObject.right * inputManager.horizontal;
            moveDirection.Normalize();//别忘记归一化

            float speed;
            if (inputManager.sprintFlag && playerStats.currentStima > 0)
            {
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                playerStats.DrianStima(sprintStimaCost * Time.deltaTime);
                playerStats.stimaRecoverTimer = 0;
            }
            else if (inputManager.walkFlag && inputManager.moveAmount > 0.2f)
            {
                speed = walkSpeed;
                playerManager.isWalking = true;
            }
            else
            {
                speed = moveSpeed;
                playerManager.isWalking = false;
                playerManager.isSprinting = false;
            }

            moveDirection *= speed;

            //将移动方向投影到当前地面，确保我们的移动方向始终垂直于地面的法线。
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            projectedVelocity.y = 0;
            rigidbody.velocity = projectedVelocity;

            if (inputManager.lockOnFlag && inputManager.sprintFlag == false)
            {
                animatorManager.UpdateAnimatiorValues(inputManager.vertical, inputManager.horizontal, playerManager.isSprinting, playerManager.isWalking);
            }
            else
            {
                //将移动的参数传给动画控制器，控制跑步动画的播放。
                animatorManager.UpdateAnimatiorValues(inputManager.moveAmount, 0, playerManager.isSprinting, playerManager.isWalking);
            }
        }

        /// <summary>
        /// 控制角色的翻滚时的朝向。
        /// </summary>
        /// <param name="delta"></param>
        public void HandleRoll(float delta)
        {
            //我们不想所有动作都能被翻滚打断，加一个判断。
            if (playerManager.isInteracting) return;
            if (playerStats.currentStima < rollingStimaCost) return;

            //如果触发了翻滚。
            if (inputManager.rollFlag)
            {
                moveDirection = cameraObject.forward * inputManager.vertical;   //计算翻滚的方向
                moveDirection += cameraObject.right * inputManager.horizontal;
                moveDirection.Normalize();

                //动量大于零，说明此时角色在移动。
                //if (inputManager.moveAmount > 0)
                //{
                if (moveDirection == Vector3.zero)
                {
                    moveDirection = cameraObject.forward;
                }
                moveDirection.y = 0;
                animatorManager.PlayTargetAnimation("Rolling", true);       //播放翻滚动画
                Quaternion rollDirection = Quaternion.LookRotation(moveDirection);
                playerTransform.rotation = rollDirection;

            }
        }

        /// <summary>
        /// 控制角色下落。
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="moveDirection"></param>
        public void HandleFall(float delta, Vector3 moveDirection)
        {
            if (playerManager.jumpFlag) return;     //方法里会强制改变角色的y轴坐标，会干扰跳跃时的模型上下移动。

            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = playerTransform.position;
            origin.y += groundDetectionRayStartPoint;   //角色Capsule Collider的底端坐标。

            //如果正前方有物体挡着，先归零移动方向。
            if (Physics.Raycast(origin, playerTransform.forward, out hit, 0.4f))
            {
                moveDirection = Vector3.zero;
            }

            //对角色施加向下的重力，以及一点点向前的力。（防止角色挂在悬崖边缘）
            if (playerManager.isInAir)
            {
                rigidbody.AddForce(-Vector3.up * fallSpeed * 1.5f);
                rigidbody.AddForce(moveDirection * fallSpeed / 3);
            }

            Vector3 dir = playerTransform.forward;
            dir.Normalize();
            origin -= dir * groundDetctionOffset;   //将射线的发射点略微偏离角色的中心。(朝着当前的移动方向进行偏离)

            targetPosition = playerTransform.position;

            Debug.DrawRay(origin, -Vector3.up, Color.red, minDistanceNeededToBeginFall, false);
            //如果这个射线命中了，就说明角色已经着地，反之则视为在空中。

            if (Physics.Raycast(origin, -Vector3.up, out hit, minDistanceNeededToBeginFall, groundCheckIgnore))
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = tp.y;        //将玩家的高度位置定位在射线撞击点处。用于走楼梯等高度差较低的情况。

                if (playerManager.isInAir)
                {
                    //当然也可以扩大地面判定的范围，提高播下落动画的门槛，但是那样门槛得提的很高，就算是0.3s的下落，距离也不短了
                    //4-5m的下落没有着地动画还挺怪的，所以还是保持高度门槛不变，确保每次都能播放着地动画。
                    if (inAirTimer > 0.5f)
                    {
                        animatorManager.PlayTargetAnimation("Land", true);
                        inAirTimer = 0f;
                    }
                    else
                    {
                        //播放一个空动画，取消下落造成的isInteracting状态。
                        animatorManager.PlayTargetAnimation("Empty", false);
                        inAirTimer = 0f;
                    }

                    playerManager.isInAir = false;
                }
            }
            else
            {   //如果在空中，即射线未命中任何物体。
                if (playerManager.isGrounded)
                {
                    playerManager.isGrounded = false;
                }

                if (playerManager.isInAir == false)
                {
                    if (playerManager.isInteracting == false)
                    {
                        animatorManager.PlayTargetAnimation("Falling", true);
                    }

                    //跌落悬崖的一瞬间给角色施加一个向着当前移动方向的力，防止其挂在悬崖边上。
                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (moveSpeed / 2);
                    rigidbody.AddForce((playerTransform.forward) * 10f, ForceMode.Impulse);//给你屁股上来一脚。
                    playerManager.isInAir = true;
                }


            }

            //处理台阶或者高度可以直接跨上去的物体。
            if (playerManager.isGrounded)
            {

                if (playerManager.isInteracting || inputManager.moveAmount > 0)
                {
                    //角色自身位置和台阶位置的插值。
                    playerTransform.position = Vector3.Lerp(playerTransform.position, targetPosition, delta * 20);
                    //playerTransform.position = Vector3.SmoothDamp(playerTransform.position, targetPosition, ref vec, Time.deltaTime / delta);
                }
                else
                {
                    playerTransform.position = targetPosition;
                }
            }
        }

        /// <summary>
        /// 控制跳跃的方法。
        /// </summary>
        /// <param name="delta"></param>
        public void HandleJump(float delta)
        {
            if (playerManager.isInteracting) return;

            if (inputManager.e_Input && inputManager.moveAmount > 0)
            {
                moveDirection = cameraObject.forward * inputManager.vertical;
                moveDirection += cameraObject.right * inputManager.horizontal;
                moveDirection.Normalize();
                moveDirection.y = 0;

                animatorManager.PlayTargetAnimation("Jump", true);
                Quaternion jumpDirection = Quaternion.LookRotation(moveDirection);
                playerTransform.rotation = jumpDirection;
            }
        }

        #endregion
    }
}