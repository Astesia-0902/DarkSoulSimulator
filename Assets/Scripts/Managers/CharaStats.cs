using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class CharaStats : MonoBehaviour
    {
        public int healthLevel = 10;
        public int stimaLevel = 10;
        public int manaLevel = 10;

        public int currentHP;
        public int maxHP;

        public int currentMana;
        public int maxMana;

        public int soulCount;

        public float currentStima;
        public int maxStima;
        public int stimaRecoverSpeed;
        public float stimaRecoverTimer;

        public bool isDead;

    }
}
