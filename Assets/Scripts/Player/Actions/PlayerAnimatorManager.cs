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
        /// 初始化动画控制器。
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
        /// 控制Animator中的参数。
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

            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);//第三个参数是“阻尼器”,即动画之间切换的缓冲。
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
        /// 用于处理动画移动以修改根运动的回调。
        /// 该回调在处理完状态机和动画后 （但在 OnAnimatorIK 之前）的每个帧中调用。
        /// 
        /// 即让存放player的空物体，随player由动画产生的移动一起移动。
        /// </summary>
        private void OnAnimatorMove()
        {
            if (playerManager.isInteracting == false)    //动画退出后，isInteracting才会被置为false
            {
                return;
            }

            float delta = Time.deltaTime;
            playerLcomotion.rigidbody.drag = 0;         //这里我们不需要drag
            Vector3 deltaPosition = anim.deltaPosition; //移动的方向就是动画造成的移动的方向

            if (playerManager.jumpFlag == false)
            {
                deltaPosition.y = 0;                        //屏蔽y轴上的移动(不需要纵向移动的动画)
            }

            Vector3 velocity = deltaPosition / delta;   //将移动速度与帧率匹配
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
