using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class UIManager : MonoBehaviour
    {
        public PlayerInventory playerInventory;
        public EquipmentWindowUI equipmentWindowUI;

        [Header("UI Windows")]
        public GameObject selectWindow;
        public GameObject HUDWindows;
        public GameObject weaponInventoryWindow;
        public GameObject EquipmentScreenWindow;

        [Header("Weapon Inventory")]
        public GameObject weaponInventorySlotPrefeb;
        public Transform WeaponInventoryParent;
        WeaponInventorySlot[] weaponInventorySlots;

        [Header("Equipment Window Slot Selected")]
        public bool rightHandSlot01Selected;
        public bool rightHandSlot02Selected;
        public bool leftHandSlot01Selected;
        public bool leftHandSlot02Selected;

        public bool inventoryFlag;
        public bool UIflag;

        InputManager inputManager;

        private void Awake()
        {
            inventoryFlag = false;
            playerInventory = FindObjectOfType<PlayerInventory>();
            inputManager = FindObjectOfType<InputManager>();
        }

        private void Start()
        {
            weaponInventorySlots = WeaponInventoryParent.GetComponentsInChildren<WeaponInventorySlot>();
            equipmentWindowUI.LoadWeaponOnEquipmentWindow(playerInventory);
        }

        private void Update()
        {
            //if (UIflag)
            //{
            //    Time.timeScale = 0;
            //}
            //else
            //{
            //    Time.timeScale = 1;
            //}

            if (inputManager.esc_Input)
            {
                //如果物品栏没有打开
                if (inventoryFlag == false)
                {
                    OpenSelectWindow();
                    UpdateUI();
                    HUDWindows.SetActive(false);
                    ActiveUIFlag();
                }
                //如果物品栏处于打开的状态
                else
                {
                    CloseSelectWindow();
                    HUDWindows.SetActive(true);
                    CloseAllInventoryWindow();
                    DeactiveUIFlag();
                }

                inventoryFlag = !inventoryFlag;
            }
        }

        /// <summary>
        /// 更新物品栏中的信息。
        /// </summary>
        public void UpdateUI()
        {
            #region Weapon Inventroy Slot
            //遍历整个物品栏。
            for (int i = 0; i < weaponInventorySlots.Length; i++)//物品栏格子的数量。
            {
                if (i < playerInventory.weaponsList.Count)//武器list的长度。
                {
                    //如果物品栏现有的格子比武器数量少，那就生成新的格子。
                    if (weaponInventorySlots.Length < playerInventory.weaponsList.Count)
                    {
                        Instantiate(weaponInventorySlotPrefeb, WeaponInventoryParent);
                        weaponInventorySlots = WeaponInventoryParent.GetComponentsInChildren<WeaponInventorySlot>();
                    }
                    weaponInventorySlots[i].AddItem(playerInventory.weaponsList[i]);
                }
                //如果物品栏现有的格子比武器数量多，就清空多余的格子。
                else
                {
                    weaponInventorySlots[i].ClearInventorySlot();
                }
            }
            #endregion
        }

        public void OpenSelectWindow()
        {
            selectWindow.SetActive(true);
        }

        private void CloseSelectWindow()
        {
            selectWindow.SetActive(false);
        }

        private void CloseAllInventoryWindow()
        {
            ResetAllSelectedSlots();
            weaponInventoryWindow.SetActive(false);
            EquipmentScreenWindow.SetActive(false);
            DeactiveUIFlag();
        }

        public void ResetAllSelectedSlots()
        {
            rightHandSlot01Selected = false;
            rightHandSlot02Selected = false;
            leftHandSlot01Selected = false;
            leftHandSlot02Selected = false;
        }

        public void ActiveUIFlag()
        {
            UIflag = true;
        }

        public void DeactiveUIFlag()
        {
            UIflag = false;
        }
    }
}
