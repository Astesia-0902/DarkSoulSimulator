using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class CombatStanceState : States
    {
        public States attackState;
        public States pursueTargetState;
        public override States Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            if (enemyManager.isInteracting)
            {
                return this;
            }
            HandleRotateTowardTarget(enemyManager);

            if (enemyManager.isPerformingAction)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0f, 0.1f, Time.deltaTime);
            }

            float distanceFromTarget = Vector3.Distance(enemyManager.transform.position, enemyManager.currentTarget.transform.position);

            if (enemyManager.currentRecoveryTime <= 0 && distanceFromTarget <= enemyManager.maxAttackRange)
            {
                return attackState;
            }
            else if (distanceFromTarget > enemyManager.maxAttackRange)
            {
                return pursueTargetState;
            }

            return this;
        }

        private void HandleRotateTowardTarget(EnemyManager enemyManager)
        {
            CharaStats currentTarget = enemyManager.currentTarget;

            //Rotate Manually
            if (enemyManager.isPerformingAction)
            {
                Vector3 direction = currentTarget.transform.position - enemyManager.transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero)
                {
                    direction = enemyManager.transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
            }
            //用NavMeshAgent控制转向
            else
            {
                ////The desired velocity of the agent including any potential contribution from avoidance.
                //Vector3 relativeRotation = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);

                ////这里本来是用root motion控制rigidbody的velocity，但是我的动画里没有rootmotion，就把NavMesh直接挂载物体上了

                //enemyManager.navMeshAgent.enabled = true;
                //enemyManager.navMeshAgent.SetDestination(currentTarget.transform.position);
                Vector3 direction = currentTarget.transform.position - enemyManager.transform.position;
                direction.y = 0;
                direction.Normalize();

                if (direction == Vector3.zero)
                {
                    direction = enemyManager.transform.forward;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
            }
            //enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
    }
}
