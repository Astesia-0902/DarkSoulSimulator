using System.Collections;
using System.Collections.Generic;
using Player.Equipments;
using UnityEngine;

namespace Astesia
{
    public class DamageCollider : MonoBehaviour
    {
        Collider damageCollider;
        public CharaManager charaManager;

        public int weaponDamage;

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        /// <summary>
        /// 启用后，接触到碰撞盒的目标就会受到伤害。
        /// </summary>
        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        /// <summary>
        /// 禁用后，接触到碰撞盒的目标就不会受到伤害。
        /// </summary>
        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerStats playerStats = other.gameObject.GetComponent<PlayerStats>();
                CharaManager playerCharaManager = other.GetComponent<CharaManager>();
                BlockCollider shield = other.GetComponentInChildren<BlockCollider>();

                if (playerCharaManager != null)
                {
                    if (playerCharaManager.isParrying)
                    {
                        charaManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Parried", true);
                        return;
                    }
                    else if (shield != null && playerCharaManager.isBlocking)
                    {
                        float physicalDamageAfterBlock = weaponDamage * (1 - shield.blockDamageAbsorption);
                        if (playerStats != null)
                        {
                            playerStats.TakeDamage(Mathf.RoundToInt(physicalDamageAfterBlock), "Hit Shield");
                        }
                        return;
                    }
                }

                if (playerStats != null)
                {
                    playerStats.TakeDamage(weaponDamage);
                }
            }

            if (other.tag == "Enemy")
            {
                EnemyStats enemyStats = other.gameObject.GetComponent<EnemyStats>();
                CharaManager enemyCharaManager = other.GetComponent<CharaManager>();

                if (enemyCharaManager != null)
                {
                    if (enemyCharaManager.isParrying)
                    {
                        charaManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("Parried", true);
                        return;
                    }
                }

                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(weaponDamage);
                }
            }
        }
    }
}