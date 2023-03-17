using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class CharaManager : MonoBehaviour
    {
        [Header("Lock On Transform")]
        public Transform lockOnTransform;
        [Header("Combat Collider")]
        public CriticalDamageCollider backStabCollider;
        public CriticalDamageCollider riposteCollider;
        [Header("Combat Flags")]
        public bool canBeRiposted;
        public bool isParrying;
        public bool canBeParried;
        public bool isBlocking;

        //???????????????????????????????
        public int pendingCriticalDamage;
    }
}
