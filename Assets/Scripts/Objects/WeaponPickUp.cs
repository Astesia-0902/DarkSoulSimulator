using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    /// <summary>
    /// ʰȡ�����Ľű������������ϣ�
    /// </summary>
    public class WeaponPickUp : Interactable
    {
        public Weapons_SO weapons_SO;
        PlayerAnimatorManager animatorController;
        //PlayerManager playerManager1;
        PlayerInventory playerInventory;
        PlayerLocomotion playerLcomotion;

        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);
            PickUpItem(playerManager);
        }

        public void PickUpItem(PlayerManager playerManager)
        {
            //��Ϊ��Һ����廥��ʱ��playerManager�Żᴫ�����壬����ֻ���������ʼ����
            animatorController = playerManager.GetComponentInChildren<PlayerAnimatorManager>();
            playerInventory = playerManager.GetComponent<PlayerInventory>();
            //playerManager1 = playerManager.GetComponent<PlayerManager>();
            playerLcomotion = playerManager.GetComponent<PlayerLocomotion>();

            playerLcomotion.rigidbody.velocity = Vector3.zero;
            animatorController.PlayTargetAnimation("Pick Up Item", true);
            playerInventory.weaponsList.Add(weapons_SO);
            playerManager.interactableUI.pickUpText.text = weapons_SO.itemName;
            playerManager.interactableUI.itemIcon.texture = weapons_SO.itemIcon.texture;
            playerManager.pickUpUI_Obj.SetActive(true);
            Destroy(gameObject);
        }
    }
}