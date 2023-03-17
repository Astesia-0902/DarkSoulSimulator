using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Astesia
{
    public class SoulCountUI : MonoBehaviour
    {
        public Text soulCountText;

        public void UpdateSoulCountText(int newSoulNumber)
        {
            soulCountText.text = newSoulNumber.ToString();
        }
    }
}
