using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Astesia
{
    public class StimaBarUI : MonoBehaviour
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

        public void SetMaxSitma(float maxStima)
        {
            slider.maxValue = maxStima;
            slider.value = maxStima;
        }

        public void SetCurrentStima(float currentStima)
        {
            slider.value = currentStima;
        }
    }
}
