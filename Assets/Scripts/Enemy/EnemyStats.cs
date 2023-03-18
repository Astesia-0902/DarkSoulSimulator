using System.Collections;
using System.Collections.Generic;
using UI.WorldSpace;
using UnityEngine;

namespace Astesia
{
    public class EnemyStats : CharaStats
    {
        EnemyAnimatorManager enemyAnimatorManager;
        EnemyManager enemyManager;

        public UIEnemyHealthBar enemyHealthBar;
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
            if (enemyHealthBar != null)
                enemyHealthBar.SetMaxHp(maxHP);
        }

        public void TakeDamage(int damage)
        {
            if (isDead) return;

            currentHP -= damage;
            if (enemyHealthBar != null)
                enemyHealthBar.SetCurrentHp(currentHP);

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
            if (enemyHealthBar != null)
                enemyHealthBar.SetCurrentHp(currentHP);

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