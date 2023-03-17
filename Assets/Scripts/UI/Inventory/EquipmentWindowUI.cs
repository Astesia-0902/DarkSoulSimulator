using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class EquipmentWindowUI : MonoBehaviour
    {
        public bool right_hand_slot_01_selected;
        public bool right_hand_slot_02_selected;
        public bool left_hand_slot_01_selected;
        public bool left_hand_slot_02_selected;

        public HandEquipmentSlotUI[] handEquipmentSlotUIs;

        private void Start()
        {
            
        }

        public void LoadWeaponOnEquipmentWindow(PlayerInventory playerInventory)
        {
            //遍历整个装备窗口里的格子，为其附上对应的武器信息。
            for (int i = 0; i < handEquipmentSlotUIs.Length; i++)
            {
                if (handEquipmentSlotUIs[i].right_hand_01)
                {
                    handEquipmentSlotUIs[i].AddItem(playerInventory.rightHandSlot[0]);
                }
                else if (handEquipmentSlotUIs[i].right_hand_02)
                {
                    handEquipmentSlotUIs[i].AddItem(playerInventory.rightHandSlot[1]);
                }
                else if (handEquipmentSlotUIs[i].left_hand_01)
                {
                    handEquipmentSlotUIs[i].AddItem(playerInventory.leftHandSlot[0]);
                }
                else if (handEquipmentSlotUIs[i].left_hand_02)
                {
                    handEquipmentSlotUIs[i].AddItem(playerInventory.leftHandSlot[1]);
                }
            }
        }

        public void SelectRightHand01()
        {
            right_hand_slot_01_selected = true;
        }

        public void SelectRightHand02()
        {
            right_hand_slot_02_selected = true;
        }

        public void SelectLeftHand01()
        {
            left_hand_slot_01_selected = true;
        }

        public void SelectLeftHand02()
        {
            left_hand_slot_02_selected = true;
        }
    }
}
