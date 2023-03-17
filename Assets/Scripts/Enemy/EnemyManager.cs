using UnityEngine;
using UnityEngine.AI;

namespace Astesia
{
    public class EnemyManager : CharaManager
    {
        public EnemyLocomotionManager enemyLocomotionManager;
        public NavMeshAgent navMeshAgent;
        public Rigidbody enemyRigidbody;
        EnemyAnimatorManager enemyAnimatorManager;
        EnemyStats enemyStats;
        EnemyManager enemyManager;

        public CharaStats currentTarget;
        public bool isPerformingAction;
        public bool isInteracting;
        public bool canDoCombo;
        public States currentState;

        [Header("Detection")]
        public float detectionRange = 15f;
        public float maxViewableAngle = 50f;
        public float minViewableAngle = -50f;
        public LayerMask detectionLayer;

        [Header("Movement")]
        public float rotationSpeed = 50f;

        [Header("Combat Stats")]
        public float currentRecoveryTime = 0;
        public float maxAttackRange = 1.5f;
        public float comboChance;

        private void Awake()
        {
            enemyManager = this;
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyRigidbody = GetComponent<Rigidbody>();
            enemyStats = GetComponent<EnemyStats>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        private void Start()
        {
            navMeshAgent.enabled = false;
            enemyRigidbody.isKinematic = false;
        }

        private void Update()
        {
            isInteracting = enemyAnimatorManager.anim.GetBool("isInteracting");
            canDoCombo = enemyAnimatorManager.anim.GetBool("canDoCombo");
            enemyAnimatorManager.anim.SetBool("isDead", enemyStats.isDead);
            HandleRecoveryTime();
        }

        private void FixedUpdate()
        {
            HandleStateMachine();
        }

        /// <summary>
        /// ????????????????
        /// </summary>
        public void HandleStateMachine()
        {
            if (currentState != null)
            {
                States nextState = currentState.Tick(enemyManager, enemyStats, enemyAnimatorManager);

                if (nextState != null)
                {
                    SwitchToNextState(nextState);
                }
            }
        }

        public void SwitchToNextState(States next)
        {
            currentState = next;
        }

        /// <summary>
        /// ?????????????????CD?????
        /// </summary>
        private void HandleRecoveryTime()
        {
            if (currentRecoveryTime > 0)
            {
                currentRecoveryTime -= Time.deltaTime;
            }

            //if (isPerformingAction)
            //{
            //    if (currentRecoveryTime <= 0)
            //    {
            //        isPerformingAction = false;
            //    }
            //}
        }

        #region Attack

        

        #endregion
    }
}
