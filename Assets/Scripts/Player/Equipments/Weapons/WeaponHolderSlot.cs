using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    /// <summary>
    /// ��ҵ���Ʒ��ۡ�һ�����ֻ��һ����Ʒ��
    /// </summary>
    public class WeaponHolderSlot : MonoBehaviour
    {
        public Transform parentOverride;//��������transform�Ľڵ����塣
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
                //��������ű����ڽ�ɫ�����ϣ���ô�����parentOverride��transform���ǽ�ɫ�ֵ�object
                //��Ϊ�������ֱ�Ӱ�transform��������ˡ�
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
            }

            currentWeaponObject = model;
        }
    }
}