using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class EnemyLocomotionManager : MonoBehaviour
    {
        EnemyManager enemyManager;
        EnemyAnimatorManager enemyAnimatorManager;

        public CapsuleCollider charaCollider;
        public CapsuleCollider combatCollider;

        private void Awake()
        {
            enemyManager = GetComponent<EnemyManager>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();

        }

        private void Start()
        {
            Physics.IgnoreCollision(charaCollider, combatCollider, true);
        }

    }
}
