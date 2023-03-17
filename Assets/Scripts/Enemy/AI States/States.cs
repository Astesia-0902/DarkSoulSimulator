using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public abstract class States : MonoBehaviour
    {
        public abstract States Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager);
    }
}
