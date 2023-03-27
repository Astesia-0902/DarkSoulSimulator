using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    /// <summary>
    /// ?????????????快??????????????????????
    /// </summary>
    public class WeaponSlotManager : MonoBehaviour
    {
        public WeaponHolderSlot leftHandHolderSlot;
        public WeaponHolderSlot rightHandHolderSlot;
        WeaponHolderSlot backHolderSlot;

        public DamageCollider leftHandDamageCollider;
        public DamageCollider rightHandDamageCollider;

        Animator anim;
        PlayerManager playerManager;
        QuickSlotUI quickSlotUI;
        PlayerInventory playerInventory;
        PlayerStats playerStats;
        PlayerLocomotion playerLcomotion;
        InputManager inputManager;

        private void Awake()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            playerStats = GetComponentInParent<PlayerStats>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            playerLcomotion = GetComponentInParent<PlayerLocomotion>();
            inputManager = GetComponentInParent<InputManager>();
            anim = GetComponent<Animator>();
            quickSlotUI = FindObjectOfType<QuickSlotUI>();
            //????????????????快????????????????????????????????????
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();

            foreach (var slot in weaponHolderSlots)
            {
                if (slot.isLeftHanded)
                {
                    leftHandHolderSlot = slot;
                }
                else if (slot.isRightHanded)
                {
                    rightHandHolderSlot = slot;
                }
                else if (slot.isBackSlot)
                {
                    backHolderSlot = slot;
                }
            }
        }

        public void LoadWeaponOnSlot(Weapons_SO weapon, bool isLeftHanded)
        {
            if (isLeftHanded)
            {
                leftHandHolderSlot.currentWeapon = weapon;
                leftHandHolderSlot.LoadWeaponModel(weapon);
                quickSlotUI.UpdateQuickSlot(isLeftHanded, weapon);

                LoadWeaponDamageColliderLeft();

                #region Left Weapon Idle
                if (weapon != null)
                {
                    anim.CrossFade(weapon.leftHandIdle, 0.2f);
                }
                else
                {
                    anim.CrossFade("Left Arm Empty", 0.2f);
                }
                #endregion
            }
            else
            {
                if (inputManager.twoHandsFlag)
                {
                    //Move left weapon to back or disable it.
                    backHolderSlot.LoadWeaponModel(leftHandHolderSlot.currentWeapon);
                    leftHandHolderSlot.UnloadAndDestroyWeapon();
                    anim.CrossFade(weapon.twoHandedIdle, 0.2f);
                }
                else
                {
                    backHolderSlot.UnloadAndDestroyWeapon();
                    #region Right Weapon Idle
                    //??????????????????????????????
                    anim.CrossFade("Two Hands Empty", 0.2f);

                    if (weapon != null)
                    {
                        anim.CrossFade(weapon.rightHandIdle, 0.2f);
                    }
                    else
                    {
                        anim.CrossFade("Right Arm Empty", 0.2f);
                    }
                    #endregion
                }

                rightHandHolderSlot.currentWeapon = weapon;
                rightHandHolderSlot.LoadWeaponModel(weapon);
                quickSlotUI.UpdateQuickSlot(isLeftHanded, weapon);

                LoadWeaponDamageColliderRight();


            }
        }

        /// <summary>
        /// ???????????????快????
        /// </summary>
        #region WeaponDamageColliders
        public void LoadWeaponDamageColliderLeft()
        {
            leftHandDamageCollider = leftHandHolderSlot.currentWeaponObject.GetComponentInChildren<DamageCollider>();
            if (leftHandDamageCollider == null)
                return;
            leftHandDamageCollider.weaponDamage = playerInventory.leftHandWeapon.baseDamage;
            leftHandDamageCollider.charaManager = GetComponentInParent<CharaManager>();
        }

        private void LoadWeaponDamageColliderRight()
        {
            rightHandDamageCollider = rightHandHolderSlot.currentWeaponObject.GetComponentInChildren<DamageCollider>();
            if (rightHandDamageCollider == null)
                return;
            rightHandDamageCollider.weaponDamage = playerInventory.rightHandWeapon.baseDamage;
            rightHandDamageCollider.charaManager = GetComponentInParent<CharaManager>();
        }

        /// <summary>
        /// ???迆?????????快??????????????
        /// </summary>
        public void EnableWeaponDamageCollider()
        {
            //??????flag?????????????????????????????????忱????enable collider??
            if (playerManager.isUsingLeftHand)
            {
                leftHandDamageCollider.EnableDamageCollider();
            }

            if (playerManager.isUsingRightHand)
            {
                rightHandDamageCollider.EnableDamageCollider();
            }
        }

        /// <summary>
        /// ???迆?????????快????????????????
        /// </summary>
        public void DisableWeaponDamageCollider()
        {
            if (leftHandDamageCollider != null)
            {
                leftHandDamageCollider.DisableDamageCollider();
            }

            if (rightHandDamageCollider != null)
            {
                rightHandDamageCollider.DisableDamageCollider();
            }
        }
        #endregion

        #region Handle Stima Costs
        public void ConsumeStimaLightAttack()
        {
            float stima = playerInventory.rightHandWeapon.baseStimaCost * playerInventory.rightHandWeapon.lightAttackStimaCoefficient;
            playerStats.DrianStima(stima);
        }

        public void ConsumeStimaTwoHandedLightAttack()
        {
            float stima = playerInventory.rightHandWeapon.baseStimaCost * playerInventory.rightHandWeapon.twoHandAttackStimaCoefficient;
            playerStats.DrianStima(stima);
        }

        public void ConsumeStimaHeavyAttack()
        {
            float stima = playerInventory.rightHandWeapon.baseStimaCost * playerInventory.rightHandWeapon.heavyAttackStimaCoefficient;
            playerStats.DrianStima(stima);
        }

        public void ConsumeStimaRolling()
        {
            float stima = playerLcomotion.rollingStimaCost;
            playerStats.DrianStima(stima);
        }
        #endregion
    }
}
