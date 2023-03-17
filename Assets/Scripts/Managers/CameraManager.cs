using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class CameraManager : MonoBehaviour
    {
        [HideInInspector]
        public Transform myTransform;    //挂载该脚本的物体的transform组件。
        [Header("Follow Obejcts")]
        public Transform targetTransform;//摄像机跟随的对象。
        public Transform cameraTransform;//摄像机本体的transform
        public Transform pivotTransform; //摄像机会绕着pivot旋转。
        [HideInInspector]
        public Vector3 cameraTransformPosition;
        private Vector3 cameraFollowVelocity = Vector3.zero;

        public LayerMask ignoreLayers;

        public static CameraManager cameraManagerSingleton;
        InputManager inputManager;
        PlayerManager playerManager;

        [Header("Camera Speed Stats")]
        public float followSpeed = 0.1f;
        public float lookSpeed = 0.01f;
        public float pivotSpeed = 0.03f;

        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        private float targetPosition;

        [Header("Thresholds")]
        public float maxPivot = 35f;
        public float minPivot = -35f;

        [Header("Collision Stats")]
        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minCollisionOffset = 0.2f;

        [Header("Lock On Stats")]
        public float maxLockOnDistance = 25f;
        public Transform nearestLockOnTarget;
        public Transform leftLockTarget;
        public Transform rightLockTarget;
        public float lockedPivotPosition = 2.2f;
        public float unlockedPivotPosition = 1.5f;
        public LayerMask environmentLayer;
        List<CharaManager> avilableLockOnTarget = new List<CharaManager>();

        public CharaManager currentLockOnTarget;
        bool lockOnInput;
        bool lockOnFlag;

        private void Awake()
        {
            cameraManagerSingleton = this;
            inputManager = FindObjectOfType<InputManager>();
            myTransform = transform;
            playerManager = FindObjectOfType<PlayerManager>();
            defaultPosition = cameraTransform.localPosition.z;//摄像机在Z轴上与holder的距离。
            ignoreLayers = ~(1 << 10 | 1 << 8 | 1 << 9 | 1 << 11 | 1 << 12);      //设置屏蔽的图层。
        }

        private void Start()
        {
            environmentLayer = LayerMask.NameToLayer("Environment");
        }

        private void Update()
        {
            lockOnFlag = inputManager.lockOnFlag;
            lockOnInput = inputManager.q_Input;
        }
        private void FixedUpdate()
        {

        }

        /// <summary>
        /// 摄像机跟随放在lateupdate里会更丝滑。
        /// </summary>
        private void LateUpdate()
        {
            float delta = Time.deltaTime;

            FollowTarget(delta);
            HandleRotation(delta, inputManager.mouseX, inputManager.mouseY);
        }

        /// <summary>
        /// 使摄像机跟随角色的方法。
        /// </summary>
        /// <param name="delta"></param>
        public void FollowTarget(float delta)
        {
            //这个插值会有锯齿感
            //Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
            Vector3 targetPosition = Vector3.SmoothDamp     //这个就丝滑很多。
                (myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
            myTransform.position = targetPosition;

            HandleCameraCollision(delta);
        }

        public void HandleRotation(float delta, float mouseX, float mouseY)
        {
            if (inputManager.lockOnFlag == false && currentLockOnTarget == null)
            {
                lookAngle += (mouseX * lookSpeed) / delta;
                pivotAngle -= (mouseY * pivotSpeed) / delta;
                pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot);

                //移动摄像机的x方向。
                Vector3 rotation = Vector3.zero;
                rotation.y = lookAngle;
                Quaternion targetRotation = Quaternion.Euler(rotation);
                myTransform.rotation = targetRotation;

                //移动摄像机的y方向。
                rotation = Vector3.zero;
                rotation.x = pivotAngle;
                targetRotation = Quaternion.Euler(rotation);
                pivotTransform.localRotation = targetRotation;  //切记这里是localrotation，如果是global的话，pivot的旋转会和holder的旋转抵消。
            }
            else if (currentLockOnTarget != null)
            {
                float distanceFromTarget = Vector3.Distance(targetTransform.position, currentLockOnTarget.transform.position);

                if (distanceFromTarget > maxLockOnDistance)
                {
                    ClearLockOnTargets();
                    inputManager.lockOnFlag = false;
                    return;
                }

                float velocity = 0;

                //摄像机应该面朝的方向。
                Vector3 dir = currentLockOnTarget.transform.position - cameraTransform.position;
                dir.Normalize();
                dir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = targetRotation;

                //TODO:暂时不清楚下面这段代码的作用，注释掉了以后也没什么区别
                dir = currentLockOnTarget.transform.position - pivotTransform.position;
                dir.Normalize();

                targetRotation = Quaternion.LookRotation(dir);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                eulerAngle.y = 0;
                pivotTransform.localEulerAngles = eulerAngle;
            }
        }

        /// <summary>
        /// 检测摄像机是否穿过了物体，并调整其位置。
        /// </summary>
        /// <param name="delta"></param>
        public void HandleCameraCollision(float delta)
        {
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - pivotTransform.position;
            direction.Normalize();

            //从pivot向摄像机射出一个指定半径的球，碰撞到物体就返回true。
            if (Physics.SphereCast(pivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
            {
                float distance = Vector3.Distance(pivotTransform.position, hit.point);
                targetPosition = -(distance - cameraCollisionOffset);   //将摄像机移动的目标点设为射线和物体碰撞点的前面一点点。
            }

            //如果碰撞点离角色实在太近，就默认摄像机卡在这个距离上。
            if (Mathf.Abs(targetPosition) < minCollisionOffset)
            {
                targetPosition = -minCollisionOffset;
            }

            //对pivot和目标点的相对位置进行插值。注意前面那个值应该是local的，不然摄像机会飞出去。
            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }

        /// <summary>
        /// 控制当前应该锁定哪个敌人的方法。
        /// TODO:目前有切换时，可锁定目标list无限增长的问题。
        /// </summary>
        /// <param name="delta"></param>
        public void HandleLockOnTarget()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceFromLeftTarget = Mathf.Infinity;
            float shortestDistanceFromRightTarget = Mathf.Infinity;
            RaycastHit hit;

            //球形范围检测周围是否有可供锁定的敌人。
            Collider[] colliders = Physics.OverlapSphere(cameraTransform.position, 26f);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharaManager chara = colliders[i].GetComponent<CharaManager>();

                if (chara != null)
                {
                    //计算锁定的方向的角度，和目标离角色的距离。
                    Vector3 lockTragetPosition = chara.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(chara.transform.position, targetTransform.position);
                    float viewableAngle = Vector3.Angle(lockTragetPosition, cameraTransform.forward);

                    //先排除主角自己的transform组件。
                    //如果不在视锥正面的一定范围内，或者距离太远，都不可以锁定。
                    if (chara.transform.root != targetTransform.transform.root
                        && viewableAngle > -50 && viewableAngle < 50
                        && distanceFromTarget <= maxLockOnDistance)
                    {
                        //判断锁定的目标和玩家之间是否有阻挡物。
                        if (Physics.Linecast(playerManager.lockOnTransform.position, chara.lockOnTransform.position, out hit, ignoreLayers))
                        {
                            Debug.DrawLine(playerManager.lockOnTransform.position, chara.lockOnTransform.position);

                            if (hit.collider.gameObject.layer == environmentLayer)
                            {

                            }
                            else
                            {
                                avilableLockOnTarget.Add(chara);
                            }
                        }
                        else
                        {
                            avilableLockOnTarget.Add(chara);
                        }
                       
                    }
                }
            }

            //判断所有敌人中，哪个离主角最近。
            for (int j = 0; j < avilableLockOnTarget.Count; j++)
            {
                float distanceFromTarget = Vector3.Distance(cameraTransform.position, avilableLockOnTarget[j].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = avilableLockOnTarget[j].lockOnTransform;
                }

                if (inputManager.lockOnFlag)
                {
                    //Transforms position from world space to local space.
                    Vector3 relativeEnemyPosition = currentLockOnTarget.transform.InverseTransformPoint(avilableLockOnTarget[j].transform.position);
                    var distanceFromLeftTarget = currentLockOnTarget.transform.position.x - avilableLockOnTarget[j].transform.position.x;
                    var distanceFromRightTarget = currentLockOnTarget.transform.position.x + avilableLockOnTarget[j].transform.position.x;

                    if (relativeEnemyPosition.x > 0f && distanceFromLeftTarget < shortestDistanceFromLeftTarget)
                    {
                        shortestDistanceFromLeftTarget = distanceFromLeftTarget;
                        leftLockTarget = avilableLockOnTarget[j].lockOnTransform;
                    }

                    if (relativeEnemyPosition.x < 0f && distanceFromRightTarget < shortestDistanceFromRightTarget)
                    {
                        shortestDistanceFromRightTarget = distanceFromRightTarget;
                        rightLockTarget = avilableLockOnTarget[j].lockOnTransform;
                    }
                }
            }
        }

        public void ClearLockOnTargets()
        {
            avilableLockOnTarget.Clear();
            nearestLockOnTarget = null;
            currentLockOnTarget = null;
        }

        public void SetCameraHeight()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition, 0);
            Vector3 newUnlockedPositon = new Vector3(0, unlockedPivotPosition, 0);

            if (currentLockOnTarget != null)
            {
                pivotTransform.transform.localPosition = Vector3.SmoothDamp(pivotTransform.transform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
            }
            else
            {
                pivotTransform.transform.localPosition = Vector3.SmoothDamp(pivotTransform.transform.localPosition, newUnlockedPositon, ref velocity, Time.deltaTime);
            }
        }
    }
}