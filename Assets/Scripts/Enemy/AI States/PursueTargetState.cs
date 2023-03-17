using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Astesia
{
    public class PursueTargetState : States
    {
        public States combatStanceState;
        public override States Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            if (enemyManager.isPerformingAction || enemyManager.isInteracting)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0f, 0.1f, Time.deltaTime);
                return this;
            }

            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            float stoppingDistance = enemyManager.maxAttackRange;
            float rotationSpeed = enemyManager.rotationSpeed;
            CharaStats currentTarget = enemyManager.currentTarget;

            #region Handle Move To Target

            Vector3 targetDirection = currentTarget.transform.position - enemyManager.transform.position;
            distanceFromTarget = Vector3.Distance(currentTarget.transform.position, enemyManager.transform.position);
            float viewableAngle = Vector3.Angle(targetDirection, enemyManager.transform.forward);

            //设置动画控制器
            if (distanceFromTarget > stoppingDistance)
            {
                //这里不知道为什么，dampTime如果设置得和玩家animator一样的话，跑步动作停下得会特别慢
                enemyAnimatorManager.anim.SetFloat("Vertical", 1f, 0.01f, Time.deltaTime);
            }
            else if (distanceFromTarget <= stoppingDistance)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0f, 0.01f, Time.deltaTime);
            }

            #endregion

            HandleRotateTowardTarget(enemyManager);

            if (distanceFromTarget <= enemyManager.maxAttackRange)
            {
                return combatStanceState;
            }

            return this;
        }

        private void HandleRotateTowardTarget(EnemyManager enemyManager)
        {
            NavMeshAgent navMeshAgent = enemyManager.navMeshAgent;
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
                transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
            }
            //用NavMeshAgent控制转向
            else
            {
                //The desired velocity of the agent including any potential contribution from avoidance.
                Vector3 relativeRotation = transform.InverseTransformDirection(navMeshAgent.desiredVelocity);

                //这里本来是用root motion控制rigidbody的velocity，但是我的动画里没有rootmotion，就把NavMesh直接挂载物体上了

                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(currentTarget.transform.position);
            }
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
    }
}
