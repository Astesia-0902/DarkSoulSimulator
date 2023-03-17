using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class EnemyStats : CharaStats
    {
        EnemyAnimatorManager enemyAnimatorManager;
        EnemyManager enemyManager;
        public int soulsAwardedOnDeath;

        private void Awake()
        {
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyManager = GetComponent<EnemyManager>();
        }

        private void Start()
        {
            maxHP = healthLevel * 10;
            currentHP = maxHP;
        }

        public void TakeDamage(int damage)
        {
            if (isDead) return;

            currentHP -= damage;

            if (currentHP <= 0)
            {
                HandleEnemyDeath();
                enemyAnimatorManager.PlayTargetAnimation("Death", true);
            }
            else
            {
                enemyAnimatorManager.PlayTargetAnimation("Hit", true);
            }
        }

        public void TakeDamageWithoutAnimation(int damage)
        {

            if (isDead)
                return;

            enemyManager.canBeRiposted = false;
            currentHP -= damage;

            if (currentHP <= 0)
            {
                HandleEnemyDeath();
            }
        }

        public void HandleEnemyDeath()
        {
            currentHP = 0;
            isDead = true;

            PlayerStats playerStats = FindObjectOfType<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.AddSoul(soulsAwardedOnDeath);
            }
        }

    }
}
