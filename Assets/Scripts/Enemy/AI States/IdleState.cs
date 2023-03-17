using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class IdleState : States
    {
        public PursueTargetState pursueTargetState;

        public override States Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            if (enemyManager.isInteracting)
            {
                return this;
            }
            #region Handle Target Detection
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRange, enemyManager.detectionLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharaStats chara = colliders[i].GetComponent<CharaStats>();

                if (chara != null)
                {
                    //Check Team ID

                    Vector3 targetDirection = chara.transform.position - transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                    if (viewableAngle > enemyManager.minViewableAngle && viewableAngle < enemyManager.maxViewableAngle)
                    {
                        enemyManager.currentTarget = chara;
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
