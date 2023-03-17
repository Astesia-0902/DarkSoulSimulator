using System;
using Astesia;
using UnityEngine;

namespace Player.Equipments
{
    public class BlockCollider : MonoBehaviour
    {
        public BoxCollider blockCollider;
        public float blockDamageAbsorption;

        private void Awake()
        {
            blockCollider = GetComponent<BoxCollider>();
        }

        public void SetBlockDamageAbsorption(Weapons_SO weaponsSo)
        {
            if (weaponsSo == null) return;
            
            blockDamageAbsorption = weaponsSo.physicalDamageAbsorption;
        }

        public void EnableBlockCollider()
        {
            blockCollider.enabled = true;
        }

        public void DisableBlockCollider()
        {
            blockCollider.enabled = false;
        }
    }
}
