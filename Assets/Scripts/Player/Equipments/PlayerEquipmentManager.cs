using System;
using Astesia;
using UnityEngine;

namespace Player.Equipments
{
    public class PlayerEquipmentManager : MonoBehaviour
    {
        private InputManager inputManager;
        public BlockCollider blockCollider;
        public PlayerInventory playerInventory;

        private void Awake()
        {
            playerInventory = GetComponentInParent<PlayerInventory>();
            inputManager = GetComponentInParent<InputManager>();
        }
        
        public void OpenBlockCollider()
        {
            if (inputManager.twoHandsFlag)
            {
                blockCollider.SetBlockDamageAbsorption(playerInventory.rightHandWeapon);
            }
            else
            {
                blockCollider.SetBlockDamageAbsorption(playerInventory.leftHandWeapon);
            }
            blockCollider.EnableBlockCollider();
        }
        
        public void CloseBlockCollider()
        {
            blockCollider.DisableBlockCollider();
        }
    }
}
