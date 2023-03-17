using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    [CreateAssetMenu(menuName ="A.I/Enemy Actions/Attack Actions")]
    public class EnemyAttackAction : EnemyAction
    {
        public bool canDoCombo;
        public EnemyAttackAction comboAttack;

        public int attackScore = 3;     //攻击触发概率的权重
        public float recoveryTime = 2f;

        public float maxAttackAngle = 35f;
        public float minAttackAngle = -35f;

        public float maxAttackRange = 2f;
        public float minAttackRange = 0f;
    }
}
