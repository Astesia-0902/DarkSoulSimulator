using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class AttackState : States
    {
        public EnemyAttackAction[] enemyAttackActions;
        public EnemyAttackAction currentAttack;

        public States combatStanceState;

        public bool willDoComboOnNextAttack;

        public override States Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            if (willDoComboOnNextAttack == false && enemyManager.isInteracting)
            {
                return this;
            }
            //如果当前是连击状态，可以不调整角度直接发动下一次攻击
            //canDoCombo用于防止下次连击的动画直接覆盖此次还没播完的动画
            else if (enemyManager.canDoCombo && willDoComboOnNextAttack)     
            {
                enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);

                if (currentAttack.canDoCombo)
                {
                    currentAttack = currentAttack.comboAttack;
                    willDoComboOnNextAttack = true;
                }
                else    //如果没有连击，就重置攻击的参数    
                {
                    enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
                    willDoComboOnNextAttack = false;
                    currentAttack = null;//重置当前的攻击，否则就会一种重复同一种攻击方式
                }

                return combatStanceState;

            }
            else if (enemyManager.isInteracting)    //防止isComboing为true，但同时也是interacting状态时，上一次攻击还没播完就被覆盖
            {
                return combatStanceState;
            }

            if (enemyManager.currentRecoveryTime > 0)
            {
                return combatStanceState;
            }

            HandleRotateTowardTarget(enemyManager);//确保攻击时也能正确面对目标

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float viewableAngle = Vector3.Angle(enemyManager.transform.forward, targetDirection);
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            if (currentAttack != null)
            {
                if (distanceFromTarget < currentAttack.minAttackRange)
                {
                    return this;
                }

                if (viewableAngle > currentAttack.minAttackAngle && viewableAngle < currentAttack.maxAttackAngle)
                {

                    //enemyManager.isPerformingAction = true;
                    enemyAnimatorManager.anim.SetFloat("Vertical", 0f, 0.01f, Time.deltaTime);
                    enemyAnimatorManager.anim.SetFloat("Horizontal", 0f, 0.01f, Time.deltaTime);
                    enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
                    //RollComboChance(enemyManager);

                    //如果当前攻击有连击，直接将下一次攻击设置为连击
                    if (currentAttack.canDoCombo)
                    {
                        currentAttack = currentAttack.comboAttack;
                        return this;
                    }
                    else
                    {
                        enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
                        willDoComboOnNextAttack = false;
                        currentAttack = null;

                        return combatStanceState;
                    }

                }
            }
            else
            {
                GetNewAttack(enemyManager);
            }

            return combatStanceState;
        }

        private void RollComboChance(EnemyManager enemyManager)
        {
            float comboChance = Random.Range(0, 100);
            if(enemyManager.canDoCombo && comboChance <= enemyManager.comboChance)
            {
                willDoComboOnNextAttack = true;
            }
        }

        /// <summary>
        /// 从所有可行的攻击手段中随机选择一种。
        /// </summary>
        private void GetNewAttack(EnemyManager enemyManager)
        {
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float viewableAngle = Vector3.Angle(enemyManager.transform.forward, targetDirection);
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            int maxScore = 0;

            //从所有攻击手段中挑选出当前情况下可以使用的攻击
            for (int i = 0; i < enemyAttackActions.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttackActions[i];

                if (distanceFromTarget <= enemyAttackAction.maxAttackRange
                    && distanceFromTarget >= enemyAttackAction.minAttackRange)
                {
                    if (viewableAngle <= enemyAttackAction.maxAttackAngle && viewableAngle >= enemyAttackAction.minAttackRange)
                    {
                        maxScore += enemyAttackAction.attackScore;
                    }
                }
            }

            int randomValue = Random.Range(0, maxScore);
            int tempScore = 0;

            //从已经挑选出来的攻击手段中随机选择一种
            for (int i = 0; i < enemyAttackActions.Length; i++)
            {
                EnemyAttackAction enemyAttackAction = enemyAttackActions[i];

                if (distanceFromTarget <= enemyAttackAction.maxAttackRange
                    && distanceFromTarget >= enemyAttackAction.minAttackRange)
                {
                    if (viewableAngle <= enemyAttackAction.maxAttackAngle && viewableAngle >= enemyAttackAction.minAttackRange)
                    {
                        if (currentAttack != null) return;

                        tempScore += enemyAttackAction.attackScore;

                        if (tempScore > randomValue)
                        {
                            currentAttack = enemyAttackAction;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 控制敌人攻击的方法
        /// </summary>
        private void AttackTarget(EnemyManager enemyManager, EnemyAnimatorManager enemyAnimatorManager)
        {
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            if (distanceFromTarget < currentAttack.minAttackRange)
            {
                return;
            }

            if (enemyManager.currentRecoveryTime <= 0 && enemyManager.isPerformingAction == false)
            {

                enemyManager.isPerformingAction = true;
                enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
                enemyAnimatorManager.anim.SetFloat("Vertical", 0f, 0.01f, Time.deltaTime);
                enemyAnimatorManager.anim.SetFloat("Horizontal", 0f, 0.01f, Time.deltaTime);
                enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
                currentAttack = null;//重置当前的攻击，否则就会一种重复同一种攻击方式
            }
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
                transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed / Time.deltaTime);
            }
            //用NavMeshAgent控制转向
            else
            {
                //The desired velocity of the agent including any potential contribution from avoidance.
                Vector3 relativeRotation = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);

                //这里本来是用root motion控制rigidbody的velocity，但是我的动画里没有rootmotion，就把NavMesh直接挂载物体上了

                enemyManager.navMeshAgent.enabled = true;
                enemyManager.navMeshAgent.SetDestination(currentTarget.transform.position);
            }
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
    }
}
