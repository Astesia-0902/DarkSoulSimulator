using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WorldSpace
{
    public class UIEnemyHealthBar : MonoBehaviour
    {
        Slider slider;
        public float activeTime = 5f;
        public float timer;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

        private void Update()
        {
            if(slider == null)
                return;
            
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                slider.gameObject.SetActive(false);
            }
            else
            {
                if (slider.gameObject.activeInHierarchy == false)
                    slider.gameObject.SetActive(true);
            }
        }

        public void SetMaxHp(int maxHp)
        {
            slider.maxValue = maxHp;
            slider.value = maxHp;
        }

        public void SetCurrentHp(int currentHp)
        {
            slider.value = currentHp;
            timer = activeTime;
            if (currentHp <= 0)
            {
                Destroy(slider.gameObject);
            }
        }
    }
}