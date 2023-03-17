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
            //������ʱ��ɫ���ᳯ����
            Vector3 rotationDirection = transform.position - playerManager.transform.position;
            rotationDirection.y = 0;
            rotationDirection.Normalize();

            Quaternion tr = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 150 * Time.deltaTime);
            playerManager.transform.rotation = targetRotation;

            animator.Play("Chest Open");
            StartCoroutine(SpawnItemInChest());
            //��ȡ��ǰ�����ڵĵ�����Ϣ
            WeaponPickUp weaponPickUp = itemSpawner.GetComponent<WeaponPickUp>();

            if (weaponPickUp != null)
            {
                weaponPickUp.weapons_SO = weaponInChest;
            }
        }

        private IEnumerator SpawnItemInChest()
        {
            yield return new WaitForSeconds(0.5f);
            //�ڱ��������ɿ�ʰȡ����
            Instantiate(itemSpawner, itemSpawnPoint.position, Quaternion.identity);
            Destroy(openChest);//�ݻٱ�����ƽű�
            gameObject.layer = 8;//�ı䱦���layer����ֹ��ҳ���̽�⵽�ɻ�����Ʒ
        }
    }
}