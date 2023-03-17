using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Astesia
{
    /// <summary>
    /// ������Ʒ����ÿ�����ӵĽű���
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
        /// ����Ʒ�����������Ʒ��
        /// </summary>
        /// <param name="newItem">��Ҫ��ӽ���Ʒ���е���Ʒ</param>
        public void AddItem(Weapons_SO newItem)
        {
            item = newItem;
            icon.sprite = newItem.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// �����Ʒ���е���Ʒ��
        /// </summary>
        public void ClearInventorySlot()
        {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// ����Ʒ����װ����Ʒ������button������
        /// </summary>
        public void EquipThisItem()
        {
            if (uiManager.rightHandSlot01Selected)
            {
                playerInventory.weaponsList.Add(playerInventory.rightHandSlot[0]);//����ǰװ�����е���Ʒ���뱳����
                playerInventory.rightHandSlot[0] = item;                          //��ǰװ��������Ʒ�滻Ϊ������ѡ�е���Ʒ��
                playerInventory.weaponsList.Remove(item);                         //�������е���Ʒ�Ƴ���
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

            //���µ�ǰ�ֳ���������Ϣ��
            playerInventory.leftHandWeapon = playerInventory.leftHandSlot[playerInventory.leftHandSlotIndex];
            playerInventory.rightHandWeapon = playerInventory.rightHandSlot[playerInventory.rightHandSlotIndex];

            //����UI
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightHandWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftHandWeapon, true);

            //����UI
            uiManager.equipmentWindowUI.LoadWeaponOnEquipmentWindow(playerInventory);
            uiManager.UpdateUI();
            uiManager.ResetAllSelectedSlots();
        }
    }
}
