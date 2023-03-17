using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class EnemyAnimatorManager : AnimatorManager
    {
        EnemyManager enemyManager;
        EnemyStats enemyStats;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            enemyStats = GetComponentInParent<EnemyStats>();
            enemyManager = GetComponentInParent<EnemyManager>();
        }

        /// <summary>
        /// 让模型跟随动画移动
        /// </summary>
        private void OnAnimatorMove()
        {
            float delta = Time.deltaTime;
            enemyManager.enemyRigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            enemyManager.enemyRigidbody.velocity = velocity;
        }
        #region Animation Event
        public override void TakeCriticalDamageAnimationEvent()
        {
            enemyStats.TakeDamageWithoutAnimation(enemyManager.pendingCriticalDamage);
            enemyManager.pendingCriticalDamage = 0;
        }

        public void EnableCanBeRiposted()
        {
            enemyManager.canBeRiposted = true;
        }

        public void DisableCanBeRiposted()
        {
            enemyManager.canBeRiposted = false;
        }

        public void EnableCombo()
        {
            anim.SetBool("canDoCombo", true);
        }

        public void DisableCombo()
        {
            anim.SetBool("canDoCombo", false);
        }

        #endregion
    }
}
