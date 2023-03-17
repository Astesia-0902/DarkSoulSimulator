using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class AmbushState : States
    {
        public bool isSleeping;
        public float detectionRange = 2f;
        public string wakeAnimation;
        public string sleepAnimation;

        public LayerMask detectionLayer;

        public States pursueTargetState;

        public override States Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            if (isSleeping && enemyManager.isInteracting == false)
            {
                enemyAnimatorManager.PlayTargetAnimation(sleepAnimation, true);
            }

            #region Handle Target Detection

            Collider[] colliders = Physics.OverlapSphere(enemyManager.transform.position, detectionRange, detectionLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharaStats charaStats = colliders[i].GetComponent<CharaStats>();

                if (charaStats != null)
                {
                    Vector3 targetDirection = charaStats.transform.position - enemyManager.transform.position;
                    float viewableAngle = Vector3.Angle(enemyManager.transform.forward, targetDirection);

                    if (viewableAngle > enemyManager.minViewableAngle 
                        && viewableAngle < enemyManager.maxAttackRange)
                    {
                        enemyManager.currentTarget = charaStats;
                        isSleeping = false;
                        enemyAnimatorManager.PlayTargetAnimation(wakeAnimation, true);
                    }
                }
            }

            #endregion

            #region Handle State Switch

            if (enemyManager.currentTarget != null)
            {
                return pursueTargetState;
            }
            else
            {
                return this;
            }

            #endregion
        }
    }
}
