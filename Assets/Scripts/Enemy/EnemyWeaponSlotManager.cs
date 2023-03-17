using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class EnemyWeaponSlotManager : MonoBehaviour
    {
        public Weapons_SO leftHandWeapon;
        public Weapons_SO rightHandWeapon;

        public WeaponHolderSlot rightHandSlot;
        public WeaponHolderSlot leftHandSlot;

        DamageCollider leftHandDmgCollider;
        DamageCollider rightHandDmgCollider;

        private void Awake()
        {
            //�����������ҵ����е�������ۡ���������۵Ľű�ֱ�ӹ��ڽ�ɫ�����ϣ�
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();

            foreach (var slot in weaponHolderSlots)
            {
                if (slot.isLeftHanded)
                {
                    leftHandSlot = slot;
                }
                else if (slot.isRightHanded)
                {
                    rightHandSlot = slot;
                }
            }
        }

        private void Start()
        {
            LoadWeaponOnBothHands();
        }

        public void LoadWeaponOnSlot(Weapons_SO weapon, bool isLeft)
        {
            if (isLeft)
            {
                leftHandSlot.currentWeapon = weapon;
                leftHandSlot.LoadWeaponModel(weapon);
                LoadWeaponDamageCollider(true);
            }
            else
            {
                rightHandSlot.currentWeapon = weapon;
                rightHandSlot.LoadWeaponModel(weapon);
                LoadWeaponDamageCollider(false);
            }
        }

        public void LoadWeaponOnBothHands()
        {
            //�Ӹ���ָ���жϣ���Ȼʹ��ֻʹ��һ������ʱ����һ���������ָ���쳣
            if (rightHandWeapon != null)
            {
                LoadWeaponOnSlot(rightHandWeapon, false);
            }

            if (leftHandWeapon != null)
            {
                LoadWeaponOnSlot(leftHandWeapon, true);
            }
        }

        public void LoadWeaponDamageCollider(bool isLeft)
        {
            if (isLeft)
            {
                leftHandDmgCollider = leftHandSlot.currentWeaponObject.GetComponentInChildren<DamageCollider>();
                leftHandDmgCollider.charaManager = GetComponentInParent<CharaManager>();
            }
            else
            {
                rightHandDmgCollider = rightHandSlot.currentWeaponObject.GetComponentInChildren<DamageCollider>();
                rightHandDmgCollider.charaManager = GetComponentInParent<CharaManager>();
            }
        }

        /// <summary>
        /// ����animation event�ͬһ����������һ��event������Ҫȷ��ʹ��ͬһ�����Ľű��£�������event���õķ�������ͬ
        /// </summary>
        #region Handle Weapon Damage Collider

        public void EnableWeaponDamageCollider()
        {
            rightHandDmgCollider.EnableDamageCollider();
        }

        public void DisableWeaponDamageCollider()
        {
            rightHandDmgCollider.DisableDamageCollider();
        }

        #endregion

        #region Handle Stima Costs
        public void ConsumeStimaLightAttack()
        {
            //float stima = playerInventory.rightHandWeapon.baseStimaCost * playerInventory.rightHandWeapon.lightAttackStimaCoefficient;
            //playerStats.DrianStima(stima);
        }

        public void ConsumeStimaTwoHandedLightAttack()
        {
            //float stima = playerInventory.rightHandWeapon.baseStimaCost * playerInventory.rightHandWeapon.twoHandAttackStimaCoefficient;
            //playerStats.DrianStima(stima);
        }

        public void ConsumeStimaHeavyAttack()
        {
            //float stima = playerInventory.rightHandWeapon.baseStimaCost * playerInventory.rightHandWeapon.heavyAttackStimaCoefficient;
            //playerStats.DrianStima(stima);
        }

        public void ConsumeStimaRolling()
        {
            //float stima = playerLcomotion.rollingStimaCost;
            //playerStats.DrianStima(stima);
        }
        #endregion

        public void EnableCombo()
        {
            //anim.SetBool("canDoCombo", true);
        }

        public void DisableCombo()
        {
            //anim.SetBool("canDoCombo", false);
        }
    }
}
