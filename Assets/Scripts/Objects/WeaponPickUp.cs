using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    /// <summary>
    /// 拾取武器的脚本（挂在武器上）
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
            //因为玩家和物体互动时，playerManager才会传给物体，所以只能在这里初始化。
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