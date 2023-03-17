using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class PlayerAnimatorManager : AnimatorManager
    {
        [HideInInspector]
        PlayerLocomotion playerLcomotion;
        PlayerManager playerManager;
        PlayerStats playerStats;

        int vertical;
        int horizontal;


        private void Awake()
        {
            
        }


        /// <summary>
        /// ��ʼ��������������
        /// </summary>
        public void Initialize()
        {
            playerLcomotion = GetComponentInParent<PlayerLocomotion>();
            playerStats = GetComponentInParent<PlayerStats>();
            playerManager = GetComponentInParent<PlayerManager>();
            anim = GetComponent<Animator>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
            canRotate = true;
        }

        /// <summary>
        /// ����Animator�еĲ�����
        /// </summary>
        /// <param name="verticalMovement"></param>
        /// <param name="horizontalMovement"></param>
        public void UpdateAnimatiorValues(float verticalMovement, float horizontalMovement, bool isSprinting,bool isWalking)
        {
            #region Vertical

            float v = 0;

            if (isSprinting)
            {
                v = 2.0f;
            }
            else if (isWalking)
            {
                v = 0.5f;
            }
            else if (verticalMovement > 0f && verticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if (verticalMovement > 0.55f)
            {
                v = 1f;
            }
            else if (verticalMovement < 0 && verticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if (verticalMovement < -0.55f)
            {
                v = -1f;
            }
            else
            {
                v = 0;
            }

            #endregion

            #region Horizontal
            float h = 0;

            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontalMovement > 0.55f)
            {
                h = 1f;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (horizontalMovement < -0.55f)
            {
                h = -1f;
            }
            else
            {
                h = 0f;
            }

            #endregion

            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);//�����������ǡ���������,������֮���л��Ļ��塣
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        public void CanRotate()
        {
            anim.SetBool("canRotate", true);
        }

        public void StopRotation()
        {
            anim.SetBool("canRotate", false);
        }

        

        /// <summary>
        /// ���ڴ������ƶ����޸ĸ��˶��Ļص���
        /// �ûص��ڴ�����״̬���Ͷ����� ������ OnAnimatorIK ֮ǰ����ÿ��֡�е��á�
        /// 
        /// ���ô��player�Ŀ����壬��player�ɶ����������ƶ�һ���ƶ���
        /// </summary>
        private void OnAnimatorMove()
        {
            if (playerManager.isInteracting == false)    //�����˳���isInteracting�Żᱻ��Ϊfalse
            {
                return;
            }

            float delta = Time.deltaTime;
            playerLcomotion.rigidbody.drag = 0;         //�������ǲ���Ҫdrag
            Vector3 deltaPosition = anim.deltaPosition; //�ƶ��ķ�����Ƕ�����ɵ��ƶ��ķ���

            if (playerManager.jumpFlag == false)
            {
                deltaPosition.y = 0;                        //����y���ϵ��ƶ�(����Ҫ�����ƶ��Ķ���)
            }

            Vector3 velocity = deltaPosition / delta;   //���ƶ��ٶ���֡��ƥ��
            playerLcomotion.rigidbody.velocity = velocity;
        }

        #region Animation Events
        public void EnableParrying()
        {
            playerManager.isParrying = true;
        }

        public void DisableParrying()
        {
            playerManager.isParrying = false;
        }

        public void EnableCombo()
        {
            anim.SetBool("canDoCombo", true);
        }

        public void DisableCombo()
        {
            anim.SetBool("canDoCombo", false);
        }

        public void EnableJumpFlag()
        {
            playerManager.jumpFlag = true;
        }

        public void DisableJumpFlag()
        {
            playerManager.jumpFlag = false;
        }

        public void EnableInvulnerable()
        {
            anim.SetBool("isInvulnerable", true);
        }

        public void DisableInvulnerable()
        {
            anim.SetBool("isInvulnerable", false);
        }

        public override void TakeCriticalDamageAnimationEvent()
        {
            playerStats.TakeDamageWithoutAnimation(playerManager.pendingCriticalDamage);
            playerManager.pendingCriticalDamage = 0;
        }
        #endregion
    }
}
