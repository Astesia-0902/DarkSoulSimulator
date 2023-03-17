using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Astesia
{
    /// <summary>
    /// ���ƽ�ɫ����״̬�Ľű���
    /// </summary>
    public class PlayerManager : CharaManager
    {
        InputManager inputManager;
        PlayerLocomotion playerLcomotion;
        Animator anim;
        PlayerAnimatorManager playerAnimatorManager;
        PlayerStats playerStats;
        CameraManager cameraManager;

        public InteractableUI interactableUI;

        public GameObject interactableUI_Obj;   //�ɻ�����Ʒ��UI��objct
        public GameObject pickUpUI_Obj;         //ʰȡ��Ʒ�����ʾUI

        [Header("Player Stats")]
        public bool isGrounded;
        public bool isInAir;
        public bool isInteracting;
        public bool isSprinting;
        public bool isWalking;
        public bool canDoCombo;
        public bool jumpFlag;
        public bool isUsingLeftHand;
        public bool isUsingRightHand;
        public bool isInvulnerable;

        private static readonly int CanRotate = Animator.StringToHash("canRotate");
        private static readonly int IsInteracting = Animator.StringToHash("isInteracting");
        private static readonly int IsInvulnerable = Animator.StringToHash("isInvulnerable");
        private static readonly int CanDoCombo = Animator.StringToHash("canDoCombo");
        private static readonly int IsUsingLeftHand = Animator.StringToHash("isUsingLeftHand");
        private static readonly int IsUsingRightHand = Animator.StringToHash("isUsingRightHand");
        private static readonly int IsDead = Animator.StringToHash("isDead");
        private static readonly int Flag = Animator.StringToHash("jumpFlag");
        private static readonly int IsInAir = Animator.StringToHash("isInAir");
        private static readonly int IsBlocking = Animator.StringToHash("isBlocking");


        private void Awake()
        {
            playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
            playerLcomotion = GetComponent<PlayerLocomotion>();
            inputManager = GetComponent<InputManager>();
            anim = GetComponentInChildren<Animator>();
            cameraManager = FindObjectOfType<CameraManager>();
            interactableUI = FindObjectOfType<InteractableUI>();
            playerStats = GetComponent<PlayerStats>();
        }

        private void Update()
        {
            isSprinting = inputManager.left_shift_Input;
            isWalking = inputManager.alt_Input;
            isBlocking = inputManager.blockFlag;
            
            playerAnimatorManager.canRotate = anim.GetBool(CanRotate);
            inputManager.isInteracting = anim.GetBool(IsInteracting);//�ýű����и�ֵ����һֱ��¼isInteracting��״̬��
            isInvulnerable = anim.GetBool(IsInvulnerable);
            canDoCombo = anim.GetBool(CanDoCombo);
            isInteracting = anim.GetBool(IsInteracting);
            isUsingLeftHand = anim.GetBool(IsUsingLeftHand);
            isUsingRightHand = anim.GetBool(IsUsingRightHand);
            
            anim.SetBool(IsDead, playerStats.isDead);
            anim.SetBool(Flag, jumpFlag);
            anim.SetBool(IsInAir, isInAir);
            anim.SetBool(IsBlocking, isBlocking);
            
            CheckInteractableObejct();
        }

        private void LateUpdate()
        {
            //���ø���ָʾ����
            inputManager.mouseLeft_Input = false;
            inputManager.mouseRight_Input = false;
            inputManager.up_Input = false;
            inputManager.down_Input = false;
            inputManager.left_Input = false;
            inputManager.right_Input = false;
            inputManager.f_Input = false;
            inputManager.e_Input = false;
            inputManager.esc_Input = false;
            inputManager.alt_Input = false;
            inputManager.left_Ctrl_Input = false;

            if (isInAir)
            {
                playerLcomotion.inAirTimer += Time.deltaTime;
            }
        }

        #region Interaction

        /// <summary>
        /// �����ǰ��û�пɻ�������ķ�����
        /// </summary>
        public void CheckInteractableObejct()
        {
            RaycastHit hit;

            //��ǰ����̽������
            if (Physics.SphereCast(transform.position, 0.1f, transform.forward, out hit, 1f, cameraManager.ignoreLayers))
            {
                if (hit.collider.CompareTag("Interactable"))
                {
                    //��ȡ�ɻ��������Interact�ű���
                    Interactable interactable = hit.collider.GetComponent<Interactable>();

                    if (interactable != null)
                    {
                        string interactText = interactable.popUpText;
                        interactableUI.interactableText.text = interactText;
                        interactableUI_Obj.SetActive(true);

                        if (inputManager.f_Input)
                        {
                            interactable.Interact(this);
                        }
                    }
                }
            }
            else
            {
                if (interactableUI_Obj != null)
                {
                    interactableUI_Obj.SetActive(false);
                }

                if (pickUpUI_Obj != null && inputManager.f_Input)
                {
                    pickUpUI_Obj.SetActive(false);
                }
            }
        }

        public void OpenChestInteraction(Transform playerStandPoint)
        {
            
        }

        #endregion
    }
}
