using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Astesia
{
    /// <summary>
    /// 控制物品栏中每个格子的脚本。
    /// </summary>
    public class WeaponInventorySlot : MonoBehaviour
    {
        PlayerInventory playerInventory;
        UIManager uiManager;
        WeaponSlotManager weaponSlotManager;

        public Image icon;
        Weapons_SO item;

        private void Awake()
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            uiManager = FindObjectOfType<UIManager>();
            weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
        }

        /// <summary>
        /// 向物品栏中添加新物品。
        /// </summary>
        /// <param name="newItem">想要添加进物品栏中的物品</param>
        public void AddItem(Weapons_SO newItem)
        {
            item = newItem;
            icon.sprite = newItem.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 清除物品栏中的物品。
        /// </summary>
        public void ClearInventorySlot()
        {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 从物品栏中装备物品。（由button触发）
        /// </summary>
        public void EquipThisItem()
        {
            if (uiManager.rightHandSlot01Selected)
            {
                playerInventory.weaponsList.Add(playerInventory.rightHandSlot[0]);//将当前装备栏中的物品加入背包。
                playerInventory.rightHandSlot[0] = item;                          //当前装备栏的物品替换为背包中选中的物品。
                playerInventory.weaponsList.Remove(item);                         //将背包中的物品移除。
            }
            else if (uiManager.rightHandSlot02Selected)
            {
                playerInventory.weaponsList.Add(playerInventory.rightHandSlot[1]);
                playerInventory.rightHandSlot[1] = item;
                playerInventory.weaponsList.Remove(item);
            }
            else if (uiManager.leftHandSlot01Selected)
            {
                playerInventory.weaponsList.Add(playerInventory.leftHandSlot[0]);
                playerInventory.leftHandSlot[0] = item;
                playerInventory.weaponsList.Remove(item);
            }
            else if (uiManager.leftHandSlot02Selected)
            {
                playerInventory.weaponsList.Add(playerInventory.leftHandSlot[1]);
                playerInventory.leftHandSlot[1] = item;
                playerInventory.weaponsList.Remove(item);
            }
            else
            {
                return;
            }

            //更新当前手持武器的信息。
            playerInventory.leftHandWeapon = playerInventory.leftHandSlot[playerInventory.leftHandSlotIndex];
            playerInventory.rightHandWeapon = playerInventory.rightHandSlot[playerInventory.rightHandSlotIndex];

            //更新UI
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightHandWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftHandWeapon, true);

            //更新UI
            uiManager.equipmentWindowUI.LoadWeaponOnEquipmentWindow(playerInventory);
            uiManager.UpdateUI();
            uiManager.ResetAllSelectedSlots();
        }
    }
}
