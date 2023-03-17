using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAnimator : StateMachineBehaviour
{
    public string targetBool;
    public bool status;
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    //animator.SetBool("isInteracting", true);
    //}
    //// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    animator.SetBool("isInteracting", false);
    //}

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(targetBool, status);
    }
}
