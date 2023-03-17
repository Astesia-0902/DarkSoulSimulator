using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class OpenChest : Interactable
    {
        public Transform playerStandPoint;
        public Transform itemSpawnPoint;
        OpenChest openChest;
        Animator animator;
        public GameObject itemSpawner;
        public Weapons_SO weaponInChest;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            openChest = GetComponent<OpenChest>();
        }

        public override void Interact(PlayerManager playerManager)
        {
            //开宝箱时角色将会朝向宝箱
            Vector3 rotationDirection = transform.position - playerManager.transform.position;
            rotationDirection.y = 0;
            rotationDirection.Normalize();

            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 150 * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;

            animator.Play("Chest Open");
            StartCoroutine(SpawnItemInChest());
            //获取当前宝箱内的道具信息
            WeaponPickUp weaponPickUp = itemSpawner.GetComponent<WeaponPickUp>();

            if (weaponPickUp != null)
            {
                weaponPickUp.weapons_SO = weaponInChest;
            }
        }

        private IEnumerator SpawnItemInChest()
        {
            yield return new WaitForSeconds(0.5f);
            //在宝箱内生成可拾取道具
            Instantiate(itemSpawner, itemSpawnPoint.position, Quaternion.identity);
            Destroy(openChest);//摧毁宝箱控制脚本
            gameObject.layer = 8;//改变宝箱的layer，防止玩家持续探测到可互动物品
        }
    }
}