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
            //�����ǰ������״̬�����Բ������Ƕ�ֱ�ӷ�����һ�ι���
            //canDoCombo���ڷ�ֹ�´������Ķ���ֱ�Ӹ��Ǵ˴λ�û����Ķ���
            else if (enemyManager.canDoCombo && willDoComboOnNextAttack)     
            {
                enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);

                if (currentAttack.canDoCombo)
                {
                    currentAttack = currentAttack.comboAttack;
                    willDoComboOnNextAttack = true;
                }
                else    //���û�������������ù����Ĳ���    
                {
                    enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
                    willDoComboOnNextAttack = false;
                    currentAttack = null;//���õ�ǰ�Ĺ���������ͻ�һ���ظ�ͬһ�ֹ�����ʽ
                }

                return combatStanceState;

            }
            else if (enemyManager.isInteracting)    //��ֹisComboingΪtrue����ͬʱҲ��interacting״̬ʱ����һ�ι�����û����ͱ�����
            {
                return combatStanceState;
            }

            if (enemyManager.currentRecoveryTime > 0)
            {
                return combatStanceState;
            }

            HandleRotateTowardTarget(enemyManager);//ȷ������ʱҲ����ȷ���Ŀ��

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

                    //�����ǰ������������ֱ�ӽ���һ�ι�������Ϊ����
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
        /// �����п��еĹ����ֶ������ѡ��һ�֡�
        /// </summary>
        private void GetNewAttack(EnemyManager enemyManager)
        {
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float viewableAngle = Vector3.Angle(enemyManager.transform.forward, targetDirection);
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            int maxScore = 0;

            //�����й����ֶ�����ѡ����ǰ����¿���ʹ�õĹ���
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

            //���Ѿ���ѡ�����Ĺ����ֶ������ѡ��һ��
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
        /// ���Ƶ��˹����ķ���
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
                currentAttack = null;//���õ�ǰ�Ĺ���������ͻ�һ���ظ�ͬһ�ֹ�����ʽ
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
            //��NavMeshAgent����ת��
            else
            {
                //The desired velocity of the agent including any potential contribution from avoidance.
                Vector3 relativeRotation = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);

                //���ﱾ������root motion����rigidbody��velocity�������ҵĶ�����û��rootmotion���Ͱ�NavMeshֱ�ӹ�����������

                enemyManager.navMeshAgent.enabled = true;
                enemyManager.navMeshAgent.SetDestination(currentTarget.transform.position);
            }
            enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, enemyManager.navMeshAgent.transform.rotation, enemyManager.rotationSpeed / Time.deltaTime);
        }
    }
}
