using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    /// <summary>
    /// 管理角色的物品栏。
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        WeaponSlotManager weaponSlotManager;
        InputManager inputManager;

        public Weapons_SO leftHandWeapon;
        public Weapons_SO rightHandWeapon;
        public Weapons_SO unarmed;
        public SpellItem_SO currentSpell;

        public Weapons_SO[] leftHandSlot = new Weapons_SO[3];
        public Weapons_SO[] rightHandSlot = new Weapons_SO[3];

        public List<Weapons_SO> weaponsList = new List<Weapons_SO>();

        [HideInInspector] public int leftHandSlotIndex = 0;
        [HideInInspector] public int rightHandSlotIndex = 0;

        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            inputManager = GetComponent<InputManager>();
        }

        private void Start()
        {
            weaponSlotManager.LoadWeaponOnSlot(leftHandSlot[leftHandSlotIndex], true);
            leftHandWeapon = leftHandSlot[leftHandSlotIndex];
            weaponSlotManager.LoadWeaponOnSlot(rightHandSlot[rightHandSlotIndex], false);
            rightHandWeapon = rightHandSlot[rightHandSlotIndex];
        }

        private void Update()
        {
            if (inputManager.left_Input)
            {
                SwitchLeftWeaponSlot();
            }
            else if (inputManager.right_Input)
            {
                SwitchRightWeaponSlot();
            }
        }

        public void SwitchLeftWeaponSlot()
        {
            leftHandSlotIndex++;
            if (leftHandSlotIndex >= leftHandSlot.Length)
            {
                leftHandSlotIndex = -1;
                weaponSlotManager.LoadWeaponOnSlot(unarmed, true);
                return;
            }

            if (leftHandSlot[leftHandSlotIndex] == null)
            {
                leftHandSlotIndex = -1;
                weaponSlotManager.LoadWeaponOnSlot(unarmed, true);
                return;
            }
            weaponSlotManager.LoadWeaponOnSlot(leftHandSlot[leftHandSlotIndex], true);
            leftHandWeapon = leftHandSlot[leftHandSlotIndex];
        }

        public void SwitchRightWeaponSlot()
        {
            rightHandSlotIndex++;
            if (rightHandSlotIndex >= rightHandSlot.Length)
            {
                rightHandSlotIndex = -1;
                weaponSlotManager.LoadWeaponOnSlot(unarmed, false);
                return;
            }

            if (rightHandSlot[rightHandSlotIndex] == null)
            {
                rightHandSlotIndex = -1;
                weaponSlotManager.LoadWeaponOnSlot(unarmed, false);
                return;
            }
            weaponSlotManager.LoadWeaponOnSlot(rightHandSlot[rightHandSlotIndex], false);
            rightHandWeapon = rightHandSlot[rightHandSlotIndex];
        }
    }
}
