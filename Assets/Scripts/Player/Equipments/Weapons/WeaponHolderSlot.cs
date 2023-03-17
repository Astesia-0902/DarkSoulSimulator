using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    /// <summary>
    /// 玩家的物品插槽。一个插槽只管一件物品。
    /// </summary>
    public class WeaponHolderSlot : MonoBehaviour
    {
        public Transform parentOverride;//控制武器transform的节点物体。
        public Weapons_SO currentWeapon;

        public bool isRightHanded;
        public bool isLeftHanded;
        public bool isBackSlot;

        public GameObject currentWeaponObject;

        public void UnloadWeapon()
        {
            if (currentWeaponObject != null)
            {
                currentWeaponObject.SetActive(false);
            }
        }

        public void UnloadAndDestroyWeapon()
        {
            if (currentWeaponObject != null)
            {
                Destroy(currentWeaponObject);
            }
        }

        public void LoadWeaponModel(Weapons_SO weaponItem)
        {
            UnloadAndDestroyWeapon();

            if (weaponItem == null)
            {
                UnloadWeapon();
                return;
            }

            GameObject model = Instantiate(weaponItem.itemPrefeb);
            if (model != null)
            {
                if (parentOverride != null)
                {
                    model.transform.parent = parentOverride;
                }
                else
                {
                    model.transform.parent = transform;
                }
                //由于这个脚本绑定在角色的手上，那么上面的parentOverride，transform就是角色手的object
                //绑定为父物体后，直接把transform归零就行了。
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
            }

            currentWeaponObject = model;
        }
    }
}