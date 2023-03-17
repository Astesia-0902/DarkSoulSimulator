using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public class AnimatorManager : MonoBehaviour
    {
        [HideInInspector]
        public Animator anim;
        public bool canRotate;

        /// <summary>
        /// 播放指定动画
        /// </summary>
        /// <param name="animationName"></param>
        /// <param name="isInteracting"></param>
        public void PlayTargetAnimation(string animationName, bool isInteracting,bool canRotate = false)
        {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("canRotate", canRotate);
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(animationName, 0.1f);
        }

        public virtual void TakeCriticalDamageAnimationEvent()
        {

        }
    }
}
