using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    /// <summary>
    /// һ�пɻ�������Ļ���
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        public string popUpText;

        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawWireSphere(transform.position, radius);
        //}

        public virtual void Interact(PlayerManager playerManager)
        {

        }
    }
}