using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Astesia
{
    /// <summary>
    /// 挂载在可互动物体的提示UI上。
    /// </summary>
    public class InteractableUI : MonoBehaviour
    {
        public Text interactableText;
        public Text pickUpText;
        public RawImage itemIcon;
    }
}
