using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Astesia
{
    public class HandEquipmentSlotUI : MonoBehaviour
    {
        public Image icon;
        Weapons_SO weapon;
        UIManager uiManager;

        public bool right_hand_01;
        public bool right_hand_02;
        public bool left_hand_01;
        public bool left_hand_02;

        private void Awake()
        {
            uiManager = FindObjectOfType<UIManager>();
        }

        public void AddItem(Weapons_SO newWeapon)
        {
            weapon = newWeapon;
            icon.sprite = weapon.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        public void ClearItem()
        {
            weapon = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        public void SelectThisSlot()
        {
            if (right_hand_01)
            {
                uiManager.rightHandSlot01Selected = true;
            }
            else if (right_hand_02)
            {
                uiManager.rightHandSlot02Selected = true;
            }
            else if (left_hand_01)
            {
                uiManager.leftHandSlot01Selected = true;
            }
            else if(left_hand_02)
            {
                uiManager.leftHandSlot02Selected = true;
            }
        }
    }
}
