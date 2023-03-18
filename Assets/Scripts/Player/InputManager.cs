using Player.Equipments;
using UnityEngine;

namespace Astesia
{
    public class InputManager : MonoBehaviour
    {
        [Header("Input Stats")]
        public float horizontal;
        public float vertical;
        public float mouseX;
        public float mouseY;
        public float moveAmount;


        public bool space_Input;
        public bool left_shift_Input;
        public bool mouseLeft_Input;
        public bool mouseRight_Input;
        public bool up_Input;
        public bool down_Input;
        public bool left_Input;
        public bool right_Input;
        public bool f_Input;
        public bool e_Input;
        public bool esc_Input;
        public bool q_Input;
        public bool tab_Input;
        public bool y_Input;
        public bool alt_Input;
        public bool mouseLeftHold_Input;
        public bool left_Ctrl_Input;
        public bool block_Input;

        public bool lockOnFlag;
        public bool rollFlag;
        public bool isInteracting;
        public bool sprintFlag;
        public bool walkFlag;
        public bool comboFlag;
        public bool twoHandsFlag;
        public bool blockFlag;

        PlayerControl inputActions;
        PlayerAnimatorManager animatorController;
        CameraManager cameraManager;
        WeaponSlotManager weaponSlotManager;
        PlayerInventory playerInventory;
        UIManager uiManager;
        PlayerEquipmentManager playerEquipmentManager;

        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            //cameraManager = CameraManager.cameraManagerSingleton;
            playerEquipmentManager = GetComponentInChildren<PlayerEquipmentManager>();
            animatorController = GetComponentInChildren<PlayerAnimatorManager>();
            cameraManager = FindObjectOfType<CameraManager>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            playerInventory = GetComponent<PlayerInventory>();
            uiManager = FindObjectOfType<UIManager>();
        }

        private void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControl();
                //这里用到了一个Lambda表达式。首先这里是注册了事件，也就是+=后面是一个方法。
                //而Lambda表达式即：实例inputActions调用一个方法，方法体为movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                //回调函数不是由该函数的实现方直接调用，而是在特定的事件或条件发生时由另外的一方调用的，用于对该事件或条件进行响应。
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
                //inputActions.PlayerActions.Sprint.performed += inputActions => left_shift_Input = inputActions.ReadValueAsButton(KeyCode.LeftShift);
                inputActions.PlayerActions.Q.performed += i => q_Input = true;
                inputActions.PlayerActions.Tab.performed += i => tab_Input = true;
                inputActions.PlayerActions.Y.performed += i => y_Input = true;
                inputActions.PlayerActions.LeftCtrl.performed += i => left_Ctrl_Input = true;
                //这个按键是长按触发的
                inputActions.PlayerActions.MouseLeftPressed.performed += i => mouseLeftHold_Input = true;
                HandleAttackInput();
                HandleEscInput();
                HandleInteractInput();
                HandleJumpInput();
                HandleSlotQuickSwitch();
            }

            inputActions.Enable();
        }

        private void Update()
        {
            //if (uiManager.UIflag == false)
            //{
                float delta = Time.deltaTime;
                TickInput(delta);               //获取控制器输入的参数
            //}
        }

        /// <summary>
        /// 把摄像机跟随放在lateUpdate里会有残影。
        /// </summary>
        private void LateUpdate()
        {

        }


        private void OnDisable()
        {
            inputActions.Disable();
        }

        /// <summary>
        /// 放在update中实时获取控制器的输入数据。
        /// </summary>
        /// <param name="delta"></param>
        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleRollInput(delta);
            HandleSprintInput(delta);
            HandleWalkInput(delta);
            HandleLockOnSwitch();
            HandleTwoHandsInput();
            //HandleBlockInput();
        }

        /// <summary>
        /// 从输入中读取各个数值的方法。
        /// </summary>
        /// <param name="delta"></param>
        void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        /// <summary>
        /// 控制翻滚的输入。
        /// </summary>
        /// <param name="delta"></param>
        public void HandleRollInput(float delta)
        {
            //检测控制翻滚的按钮是否已经被按下。
            space_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            rollFlag = space_Input;//持续重置Flag

            if (space_Input)
            {
                rollFlag = true;
            }
        }

        public void HandleWalkInput(float delta)
        {
            alt_Input = inputActions.PlayerActions.Walk.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            walkFlag = alt_Input;//持续重置Flag

            if (alt_Input)
            {
                walkFlag = true;
            }
        }

        public void HandleSprintInput(float delta)
        {
            left_shift_Input = inputActions.PlayerActions.Sprint.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
            sprintFlag = left_shift_Input;//持续重置Flag

            // if (left_shift_Input)
            // {
            //     sprintFlag = true;
            // }
        }

        public void HandleAttackInput()
        {
            inputActions.PlayerActions.MouseLeft.performed += i => mouseLeft_Input = true;
            inputActions.PlayerActions.MouseRight.performed += i => mouseRight_Input = true;
            inputActions.PlayerActions.MouseRight.canceled += i =>
            {
                blockFlag = false;
                //Diactivate the collider when the player is not blocking.
                playerEquipmentManager.CloseBlockCollider();
            };
        }

        public void HandleBlockInput()
        {
            block_Input = inputActions.PlayerActions.MouseRight.phase ==
                          UnityEngine.InputSystem.InputActionPhase.Performed;
        } 

        public void HandleSlotQuickSwitch()
        {
            inputActions.PlayerActions.Left.performed += i => left_Input = true;
            inputActions.PlayerActions.Right.performed += i => right_Input = true;
        }

        public void HandleInteractInput()
        {
            inputActions.PlayerActions.F.performed += i => f_Input = true;
        }

        public void HandleJumpInput()
        {
            inputActions.PlayerActions.E.performed += i => e_Input = true;
        }

        public void HandleEscInput()
        {
            inputActions.PlayerActions.Esc.performed += i => esc_Input = true;
        }

        bool right_to_Left_Flag = false;
        /// <summary>
        /// 控制视角锁定的方法。
        /// </summary>
        public void HandleLockOnSwitch()
        {
            if (q_Input && lockOnFlag == false)
            {
                q_Input = false;
                cameraManager.HandleLockOnTarget();

                if (cameraManager.nearestLockOnTarget != null)
                {
                    cameraManager.currentLockOnTarget = cameraManager.nearestLockOnTarget;
                    lockOnFlag = true;
                }
            }
            else if (q_Input && lockOnFlag)
            {
                lockOnFlag = false;
                q_Input = false;
                cameraManager.ClearLockOnTargets();
            }

            //在多个目标之间切换锁定
            //TODO:目前有切换时，可锁定目标list无限增长的问题。
            if (lockOnFlag && tab_Input)
            {
                tab_Input = false;
                cameraManager.HandleLockOnTarget();

                if (right_to_Left_Flag == false)
                {
                    if (cameraManager.rightLockTarget != null)
                    {
                        cameraManager.currentLockOnTarget = cameraManager.rightLockTarget;
                    }
                    else
                    {
                        right_to_Left_Flag = true;
                    }
                }
                else
                {
                    if (cameraManager.leftLockTarget != null)
                    {
                        cameraManager.currentLockOnTarget = cameraManager.leftLockTarget;
                    }
                    else
                    {
                        right_to_Left_Flag = false;
                    }
                }
            }

            cameraManager.SetCameraHeight();
        }

        public void HandleTwoHandsInput()
        {
            if (y_Input)
            {
                y_Input = false;
                twoHandsFlag = !twoHandsFlag;

                //双手持握武器时，仅加载右手的武器。
                if (twoHandsFlag)
                {
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightHandWeapon, false);
                }
                else
                {
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightHandWeapon, false);
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftHandWeapon, true);
                }
            }
        }
    }
}