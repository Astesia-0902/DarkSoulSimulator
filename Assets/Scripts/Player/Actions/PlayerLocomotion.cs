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
        public GameObject normalCamera;     //TODO:֮�󻹻��и�����Ŀ��������

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
            HandleRoll(delta);     //���Ʒ���

        }

        /// <summary>
        /// ��RigidBody�йصĽ������FixedUpdate�С�
        /// </summary>
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            HandleMovement(delta);          //�����ƶ�
            HandleFall(delta, moveDirection);
            HandleRotation(delta);
        }

        #region Movement

        Vector3 normalVector;
        Vector3 targetPosition;

        /// <summary>
        /// ���ܶ�ʱ���ƽ�ɫģ�͵ĳ���
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

            //����Ŀ��ʱ����ɫ��ʼ������Ŀ�ꡣ
            if (inputManager.lockOnFlag && inputManager.sprintFlag == false)
            {
                //��̺ͷ���ʱ����Ȼ������ָ���ķ���
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
            else    //û������Ŀ��ʱ
            {
                Vector3 targetDir = Vector3.zero;
                float moveOverride = inputManager.moveAmount;

                targetDir = inputManager.vertical * cameraObject.forward;      //���xy�������ϵ��ƶ�����
                targetDir += inputManager.horizontal * cameraObject.right;     //ע�������Ǹ���������ķ�����Ϊ�Ǹ����ƶ�����ת�򣬶���ҵ��ƶ������ǻ���������ķ���

                targetDir.Normalize();
                targetDir.y = 0;

                //û�������Ĭ����ǰ�ܡ�
                if (targetDir == Vector3.zero)
                {
                    targetDir = transform.forward;
                }

                float rs = rotationSpeed;

                Quaternion tr = Quaternion.LookRotation(targetDir);     //����תĿ������ΪĿ�귽��
                Quaternion targetRotation = Quaternion.Slerp(playerTransform.rotation, tr, delta * rs);

                playerTransform.rotation = targetRotation;
            }
        }

        /// <summary>
        /// ���ƽ�ɫ���ƶ���
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

            moveDirection = cameraObject.forward * inputManager.vertical;   //ע�������Ǹ���������ķ���
            moveDirection += cameraObject.right * inputManager.horizontal;
            moveDirection.Normalize();//�����ǹ�һ��

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

            //���ƶ�����ͶӰ����ǰ���棬ȷ�����ǵ��ƶ�����ʼ�մ�ֱ�ڵ���ķ��ߡ�
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            projectedVelocity.y = 0;
            rigidbody.velocity = projectedVelocity;

            if (inputManager.lockOnFlag && inputManager.sprintFlag == false)
            {
                animatorManager.UpdateAnimatiorValues(inputManager.vertical, inputManager.horizontal, playerManager.isSprinting, playerManager.isWalking);
            }
            else
            {
                //���ƶ��Ĳ������������������������ܲ������Ĳ��š�
                animatorManager.UpdateAnimatiorValues(inputManager.moveAmount, 0, playerManager.isSprinting, playerManager.isWalking);
            }
        }

        /// <summary>
        /// ���ƽ�ɫ�ķ���ʱ�ĳ���
        /// </summary>
        /// <param name="delta"></param>
        public void HandleRoll(float delta)
        {
            //���ǲ������ж������ܱ�������ϣ���һ���жϡ�
            if (playerManager.isInteracting) return;
            if (playerStats.currentStima < rollingStimaCost) return;

            //��������˷�����
            if (inputManager.rollFlag)
            {
                moveDirection = cameraObject.forward * inputManager.vertical;   //���㷭���ķ���
                moveDirection += cameraObject.right * inputManager.horizontal;
                moveDirection.Normalize();

                //���������㣬˵����ʱ��ɫ���ƶ���
                //if (inputManager.moveAmount > 0)
                //{
                if (moveDirection == Vector3.zero)
                {
                    moveDirection = cameraObject.forward;
                }
                moveDirection.y = 0;
                animatorManager.PlayTargetAnimation("Rolling", true);       //���ŷ�������
                Quaternion rollDirection = Quaternion.LookRotation(moveDirection);
                playerTransform.rotation = rollDirection;

            }
        }

        /// <summary>
        /// ���ƽ�ɫ���䡣
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="moveDirection"></param>
        public void HandleFall(float delta, Vector3 moveDirection)
        {
            if (playerManager.jumpFlag) return;     //�������ǿ�Ƹı��ɫ��y�����꣬�������Ծʱ��ģ�������ƶ���

            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = playerTransform.position;
            origin.y += groundDetectionRayStartPoint;   //��ɫCapsule Collider�ĵ׶����ꡣ

            //�����ǰ�������嵲�ţ��ȹ����ƶ�����
            if (Physics.Raycast(origin, playerTransform.forward, out hit, 0.4f))
            {
                moveDirection = Vector3.zero;
            }

            //�Խ�ɫʩ�����µ��������Լ�һ�����ǰ����������ֹ��ɫ�������±�Ե��
            if (playerManager.isInAir)
            {
                rigidbody.AddForce(-Vector3.up * fallSpeed * 1.5f);
                rigidbody.AddForce(moveDirection * fallSpeed / 3);
            }

            Vector3 dir = playerTransform.forward;
            dir.Normalize();
            origin -= dir * groundDetctionOffset;   //�����ߵķ������΢ƫ���ɫ�����ġ�(���ŵ�ǰ���ƶ��������ƫ��)

            targetPosition = playerTransform.position;

            Debug.DrawRay(origin, -Vector3.up, Color.red, minDistanceNeededToBeginFall, false);
            //���������������ˣ���˵����ɫ�Ѿ��ŵأ���֮����Ϊ�ڿ��С�

            if (Physics.Raycast(origin, -Vector3.up, out hit, minDistanceNeededToBeginFall, groundCheckIgnore))
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = tp.y;        //����ҵĸ߶�λ�ö�λ������ײ���㴦��������¥�ݵȸ߶Ȳ�ϵ͵������

                if (playerManager.isInAir)
                {
                    //��ȻҲ������������ж��ķ�Χ����߲����䶯�����ż������������ż�����ĺܸߣ�������0.3s�����䣬����Ҳ������
                    //4-5m������û���ŵض�����ͦ�ֵģ����Ի��Ǳ��ָ߶��ż����䣬ȷ��ÿ�ζ��ܲ����ŵض�����
                    if (inAirTimer > 0.5f)
                    {
                        animatorManager.PlayTargetAnimation("Land", true);
                        inAirTimer = 0f;
                    }
                    else
                    {
                        //����һ���ն�����ȡ��������ɵ�isInteracting״̬��
                        animatorManager.PlayTargetAnimation("Empty", false);
                        inAirTimer = 0f;
                    }

                    playerManager.isInAir = false;
                }
            }
            else
            {   //����ڿ��У�������δ�����κ����塣
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

                    //�������µ�һ˲�����ɫʩ��һ�����ŵ�ǰ�ƶ������������ֹ��������±��ϡ�
                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (moveSpeed / 2);
                    rigidbody.AddForce((playerTransform.forward) * 10f, ForceMode.Impulse);//����ƨ������һ�š�
                    playerManager.isInAir = true;
                }


            }

            //����̨�׻��߸߶ȿ���ֱ�ӿ���ȥ�����塣
            if (playerManager.isGrounded)
            {

                if (playerManager.isInteracting || inputManager.moveAmount > 0)
                {
                    //��ɫ����λ�ú�̨��λ�õĲ�ֵ��
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
        /// ������Ծ�ķ�����
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