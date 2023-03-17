using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Astesia
{
    public class ManaBarUI : MonoBehaviour
    {
        [HideInInspector]
        public Slider slider;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

        private void Update()
        {

        }

        public void SetMaxMana(float maxMana)
        {
            slider.maxValue = maxMana;
            slider.value = maxMana;
        }

        public void SetCurrentMana(float currentMana)
        {
            slider.value = currentMana;
        }
    }
}