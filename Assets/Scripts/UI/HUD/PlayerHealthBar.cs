using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Astesia
{
    public class PlayerHealthBar : MonoBehaviour
    {
        [HideInInspector]
        public Slider slider;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

        public void SetMaxHP(int maxHP)
        {
            slider.maxValue = maxHP;
            slider.value = maxHP;
        }

        public void SetCurrentHP(int currentHP)
        {
            slider.value = currentHP;
        }
    }
}