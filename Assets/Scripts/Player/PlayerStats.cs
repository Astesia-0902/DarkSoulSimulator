using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class PlayerStats : CharaStats
    {

        public PlayerHealthBar playerHealthBar;
        public StimaBarUI stimaBarUI;
        public ManaBarUI manaBarUI;
        public SoulCountUI soulCountUI;
        PlayerManager playerManager;
        PlayerAnimatorManager animatorController;

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();
            animatorController = GetComponentInChildren<PlayerAnimatorManager>();
        }

        private void Start()
        {
            maxHP = healthLevel * 10;//????????????????????????
            maxStima = stimaLevel * 10;
            maxMana = manaLevel * 10;
            currentMana = maxMana;
            currentHP = maxHP;
            currentStima = maxStima;
            manaBarUI.SetMaxMana(maxMana);
            playerHealthBar.SetMaxHP(maxHP);
            stimaBarUI.SetMaxSitma(maxStima);
        }

        private void Update()
        {
            RegenerateStima();
        }

        private void RegenerateStima()
        {
            if (playerManager.isInteracting)
            {
                stimaRecoverTimer = 0;
                return;
            }

            stimaRecoverTimer += Time.deltaTime;
            
            if (currentStima < maxStima && stimaRecoverTimer >= 1f)
            {
                currentStima += stimaRecoverSpeed * Time.deltaTime;
                stimaBarUI.SetCurrentStima(currentStima);
            }
            else if (currentStima >= maxStima)
            {
                currentStima = maxStima;
            }
        }

        public void TakeDamage(int damage,string damageAnimation = "Hit")
        {
            if (playerManager.isInvulnerable)
                return;

            if (isDead)
                return;

            currentHP -= damage;
            playerHealthBar.SetCurrentHP(currentHP);

            if (currentHP <= 0)
            {
                currentHP = 0;
                isDead = true;
                animatorController.PlayTargetAnimation("Death", true);
            }
            else
            {
                animatorController.PlayTargetAnimation(damageAnimation, true);
            }
        }

        public void TakeDamageWithoutAnimation(int damage)
        {
            if (playerManager.isInvulnerable)
                return;

            if (isDead)
                return;

            currentHP -= damage;
            playerHealthBar.SetCurrentHP(currentHP);

            if (currentHP <= 0)
            {
                currentHP = 0;
                isDead = true;
            }
        }

        public void DrianStima(float stima)
        {
            currentStima -= stima;
            if (currentStima <= 0)
            {
                currentStima = 0;
            }
            stimaBarUI.SetCurrentStima(currentStima);
        }

        public void DrainMana(int mana)
        {
            currentMana -= mana;
            if (currentMana < 0)
            {
                currentMana = 0;
            }
            manaBarUI.SetCurrentMana(currentMana);
        }

        public void HealChara(int healingAmount)
        {
            currentHP += healingAmount;
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }

            playerHealthBar.SetCurrentHP(currentHP);
        }

        public void AddSoul(int soul)
        {
            soulCount += soul;
            soulCountUI.UpdateSoulCountText(soulCount);
        }
    }
}
