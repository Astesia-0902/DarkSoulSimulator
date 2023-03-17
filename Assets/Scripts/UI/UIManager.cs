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
                //�����Ʒ��û�д�
                if (inventoryFlag == false)
                {
                    OpenSelectWindow();
                    UpdateUI();
                    HUDWindows.SetActive(false);
                    ActiveUIFlag();
                }
                //�����Ʒ�����ڴ򿪵�״̬
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
        /// ������Ʒ���е���Ϣ��
        /// </summary>
        public void UpdateUI()
        {
            #region Weapon Inventroy Slot
            //����������Ʒ����
            for (int i = 0; i < weaponInventorySlots.Length; i++)//��Ʒ�����ӵ�������
            {
                if (i < playerInventory.weaponsList.Count)//����list�ĳ��ȡ�
                {
                    //�����Ʒ�����еĸ��ӱ����������٣��Ǿ������µĸ��ӡ�
                    if (weaponInventorySlots.Length < playerInventory.weaponsList.Count)
                    {
                        Instantiate(weaponInventorySlotPrefeb, WeaponInventoryParent);
                        weaponInventorySlots = WeaponInventoryParent.GetComponentsInChildren<WeaponInventorySlot>();
                    }
                    weaponInventorySlots[i].AddItem(playerInventory.weaponsList[i]);
                }
                //�����Ʒ�����еĸ��ӱ����������࣬����ն���ĸ��ӡ�
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
