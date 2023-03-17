using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class CameraManager : MonoBehaviour
    {
        [HideInInspector]
        public Transform myTransform;    //���ظýű��������transform�����
        [Header("Follow Obejcts")]
        public Transform targetTransform;//���������Ķ���
        public Transform cameraTransform;//����������transform
        public Transform pivotTransform; //�����������pivot��ת��
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
            defaultPosition = cameraTransform.localPosition.z;//�������Z������holder�ľ��롣
            ignoreLayers = ~(1 << 10 | 1 << 8 | 1 << 9 | 1 << 11 | 1 << 12);      //�������ε�ͼ�㡣
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
        /// ������������lateupdate����˿����
        /// </summary>
        private void LateUpdate()
        {
            float delta = Time.deltaTime;

            FollowTarget(delta);
            HandleRotation(delta, inputManager.mouseX, inputManager.mouseY);
        }

        /// <summary>
        /// ʹ����������ɫ�ķ�����
        /// </summary>
        /// <param name="delta"></param>
        public void FollowTarget(float delta)
        {
            //�����ֵ���о�ݸ�
            //Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
            Vector3 targetPosition = Vector3.SmoothDamp     //�����˿���ܶࡣ
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

                //�ƶ��������x����
                Vector3 rotation = Vector3.zero;
                rotation.y = lookAngle;
                Quaternion targetRotation = Quaternion.Euler(rotation);
                myTransform.rotation = targetRotation;

                //�ƶ��������y����
                rotation = Vector3.zero;
                rotation.x = pivotAngle;
                targetRotation = Quaternion.Euler(rotation);
                pivotTransform.localRotation = targetRotation;  //�м�������localrotation�������global�Ļ���pivot����ת���holder����ת������
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

                //�����Ӧ���泯�ķ���
                Vector3 dir = currentLockOnTarget.transform.position - cameraTransform.position;
                dir.Normalize();
                dir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = targetRotation;

                //TODO:��ʱ�����������δ�������ã�ע�͵����Ժ�Ҳûʲô����
                dir = currentLockOnTarget.transform.position - pivotTransform.position;
                dir.Normalize();

                targetRotation = Quaternion.LookRotation(dir);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                eulerAngle.y = 0;
                pivotTransform.localEulerAngles = eulerAngle;
            }
        }

        /// <summary>
        /// ���������Ƿ񴩹������壬��������λ�á�
        /// </summary>
        /// <param name="delta"></param>
        public void HandleCameraCollision(float delta)
        {
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - pivotTransform.position;
            direction.Normalize();

            //��pivot����������һ��ָ���뾶������ײ������ͷ���true��
            if (Physics.SphereCast(pivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
            {
                float distance = Vector3.Distance(pivotTransform.position, hit.point);
                targetPosition = -(distance - cameraCollisionOffset);   //��������ƶ���Ŀ�����Ϊ���ߺ�������ײ���ǰ��һ��㡣
            }

            //�����ײ�����ɫʵ��̫������Ĭ�������������������ϡ�
            if (Mathf.Abs(targetPosition) < minCollisionOffset)
            {
                targetPosition = -minCollisionOffset;
            }

            //��pivot��Ŀ�������λ�ý��в�ֵ��ע��ǰ���Ǹ�ֵӦ����local�ģ���Ȼ�������ɳ�ȥ��
            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;
        }

        /// <summary>
        /// ���Ƶ�ǰӦ�������ĸ����˵ķ�����
        /// TODO:Ŀǰ���л�ʱ��������Ŀ��list�������������⡣
        /// </summary>
        /// <param name="delta"></param>
        public void HandleLockOnTarget()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceFromLeftTarget = Mathf.Infinity;
            float shortestDistanceFromRightTarget = Mathf.Infinity;
            RaycastHit hit;

            //���η�Χ�����Χ�Ƿ��пɹ������ĵ��ˡ�
            Collider[] colliders = Physics.OverlapSphere(cameraTransform.position, 26f);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharaManager chara = colliders[i].GetComponent<CharaManager>();

                if (chara != null)
                {
                    //���������ķ���ĽǶȣ���Ŀ�����ɫ�ľ��롣
                    Vector3 lockTragetPosition = chara.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(chara.transform.position, targetTransform.position);
                    float viewableAngle = Vector3.Angle(lockTragetPosition, cameraTransform.forward);

                    //���ų������Լ���transform�����
                    //���������׶�����һ����Χ�ڣ����߾���̫Զ����������������
                    if (chara.transform.root != targetTransform.transform.root
                        && viewableAngle > -50 && viewableAngle < 50
                        && distanceFromTarget <= maxLockOnDistance)
                    {
                        //�ж�������Ŀ������֮���Ƿ����赲�
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

            //�ж����е����У��ĸ������������
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