using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Astesia
{
    public class QuickSlotUI : MonoBehaviour
    {
        public Image rightSlotIcon;
        public Image leftSlotIcon;

        public void UpdateQuickSlot(bool isLeft,Weapons_SO weapons)
        {
            if (isLeft)
            {
                if (weapons.itemIcon != null)
                {
                    leftSlotIcon.sprite = weapons.itemIcon;
                    leftSlotIcon.enabled = true;
                }
                else
                {
                    leftSlotIcon.sprite = null;
                    leftSlotIcon.enabled = false;
                }
            }
            else
            {
                if (weapons.itemIcon != null)
                {
                    rightSlotIcon.sprite = weapons.itemIcon;
                    rightSlotIcon.enabled = true;
                }
                else
                {
                    rightSlotIcon.sprite = null;
                    rightSlotIcon.enabled = false;
                }
            }
        }
    }
}
