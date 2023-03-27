using Astesia;
using Player.Equipments;
using UnityEngine;

namespace Player.Actions
{
    public class PlayerAttack : MonoBehaviour
    {
        PlayerEquipmentManager equipmentManager;
        PlayerAnimatorManager animatorController;
        InputManager inputManager;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        WeaponSlotManager weaponSlotManager;
        Camera camera;
        PlayerStats playerStats;

        public Transform criticalAttackRaycastPoint;

        LayerMask backStabLayer = 1 << 12 | 1 << 9;
        LayerMask riposteLayer = 1 << 13;

        public UIManager uiManager;

        public int comboCount;
        public float comboTime;
        float comboTimeBuffer;

        private void Awake()
        {
            equipmentManager = GetComponent<PlayerEquipmentManager>();
            animatorController = GetComponent<PlayerAnimatorManager>();
            inputManager = GetComponentInParent<InputManager>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            playerManager = GetComponentInParent<PlayerManager>();
            playerStats = GetComponentInParent<PlayerStats>();
            weaponSlotManager = GetComponent<WeaponSlotManager>();
            camera = Camera.main;
            comboCount = 0;
        }

        private void Update()
        {
            HandleMouseLeftInput();
            HandleCriticalAttack();
            ResetComboCount();
            HandleMouseRightInput();
            HandleLeftCtrlInput();
        }

        #region Input Actions

        private void HandleMouseLeftInput()
        {
            if (inputManager.mouseLeft_Input)
            {
                switch (playerInventory.rightHandWeapon.weaponType)
                {
                    case WeaponType.MeleeWeapon:
                        PerformMouseLeftMeleeAction();
                        break;

                    default:
                        PerformMouseLeftCastAction(playerInventory.currentSpell);
                        break;
                }
            }
        }

        private void HandleLeftCtrlInput()
        {
            if (playerManager.isInteracting)
                return;

            if (inputManager.left_Ctrl_Input)
            {
                PerformWeaponArt(true);
            }
        }

        private void HandleMouseRightInput()
        {
            if (playerManager.isInteracting)
                return;

            if (inputManager.mouseRight_Input)
            {
                animatorController.anim.SetBool("isUsingLeftHand", true);
                switch (playerInventory.leftHandWeapon.weaponType)
                {
                    case WeaponType.MeleeWeapon:
                        HeavyAttack(playerInventory.leftHandWeapon);
                        break;
                    case WeaponType.Shield:
                        PerformBlockAction();
                        break;

                    default:
                        PerformMouseLeftCastAction(playerInventory.currentSpell);
                        break;
                }
            }
            else
            {
                animatorController.anim.SetBool("isUsingLeftHand", false);
                playerManager.isBlocking = false;
            }
        }

        private void HandleCriticalAttack()
        {
            if (inputManager.mouseLeftHold_Input)
            {
                inputManager.mouseLeftHold_Input = false;
                HandleBackStab();
            }
        }

        #endregion

        #region Attack Actions

        public void PerformMouseLeftMeleeAction()
        {
            //canDoCombo会在Animation Event中触发。
            if (playerManager.canDoCombo)
            {
                inputManager.comboFlag = true;
                animatorController.anim.SetBool("isUsingRightHand", true);
                HandleCombo(playerInventory.rightHandWeapon);
                inputManager.comboFlag = false; //确保一次攻击只会触发一次连击。
            }
            else
            {
                if (playerManager.isInteracting)
                    return;
                if (playerManager.canDoCombo) return; //确保一次攻击只会触发一次连击。
                animatorController.anim.SetBool("isUsingRightHand", true);
                LightAttack(playerInventory.rightHandWeapon);
            }
        }

        public void PerformMouseLeftCastAction(SpellItem_SO spellItem_SO)
        {
            if (playerManager.isInteracting)
                return;

            if (playerStats.currentMana < spellItem_SO.manaCost)
            {
                animatorController.PlayTargetAnimation("Shrug", true);
                return;
            }

            if (playerInventory.currentSpell != null)
            {
                switch (spellItem_SO.spellType)
                {
                    case SpellType.FaithSpell:
                        playerInventory.currentSpell.SpellCasting(animatorController, playerStats, weaponSlotManager);
                        break;

                    case SpellType.MagicSpell:
                        break;

                    case SpellType.PyroSpell:
                        playerInventory.currentSpell.SpellCasting(animatorController, playerStats, weaponSlotManager);
                        break;
                }
            }
        }

        public void SuccessfullyCasted()
        {
            playerInventory.currentSpell.SpellCasted(animatorController, playerStats);
        }

        /// <summary>
        /// 处理连击释放的方法。
        /// </summary>
        /// <param name="weapons">当前使用的武器SO</param>
        public void HandleCombo(Weapons_SO weapons)
        {
            if (inputManager.twoHandsFlag)
            {
                if (playerStats.currentStima < weapons.baseStimaCost * weapons.twoHandAttackStimaCoefficient) return;

                if (inputManager.comboFlag)
                {
                    animatorController.anim.SetBool("canDoCombo", false);
                    if (comboCount > 0 && weapons.attackNames_TwoHanded[comboCount] != null)
                    {
                        animatorController.PlayTargetAnimation(weapons.attackNames_TwoHanded[comboCount], true);
                        comboTimeBuffer = comboTime;
                        comboCount++;
                    }
                }

                //如果当前是连招的最后一击，则重置连招计数器。
                if (weapons.attackNames_TwoHanded[comboCount] == null)
                {
                    comboCount = 0;
                }
            }
            else
            {
                if (playerStats.currentStima < weapons.baseStimaCost * weapons.lightAttackStimaCoefficient) return;

                if (inputManager.comboFlag)
                {
                    animatorController.anim.SetBool("canDoCombo", false);
                    if (comboCount > 0 && weapons.attackNames[comboCount] != null)
                    {
                        animatorController.PlayTargetAnimation(weapons.attackNames[comboCount], true);
                        comboTimeBuffer = comboTime;
                        comboCount++;
                    }
                }

                //如果当前是连招的最后一击，则重置连招计数器。
                if (weapons.attackNames[comboCount] == null)
                {
                    comboCount = 0;
                }
            }
        }

        /// <summary>
        /// 普通攻击的第一下。
        /// </summary>
        /// <param name="weapons">当前使用的武器的SO</param>
        public void LightAttack(Weapons_SO weapons)
        {
            //if (uiManager.UIflag) return;
            comboCount = 0;

            if (inputManager.twoHandsFlag)
            {
                if (weapons.attackNames_TwoHanded[0] == "") return;
                if (playerStats.currentStima < weapons.baseStimaCost * weapons.twoHandAttackStimaCoefficient) return;
                animatorController.PlayTargetAnimation(weapons.attackNames_TwoHanded[0], true);
            }
            else
            {
                if (weapons.attackNames[0] == "") return;
                if (playerStats.currentStima < weapons.baseStimaCost * weapons.lightAttackStimaCoefficient) return;
                animatorController.PlayTargetAnimation(weapons.attackNames[0], true);
            }

            comboTimeBuffer = comboTime;
            comboCount++;
        }

        public void HeavyAttack(Weapons_SO weapons)
        {
            //if (uiManager.UIflag) return;
            if (inputManager.twoHandsFlag)
            {
            }
            else
            {
                if (weapons.One_Hand_Heavy_Attack_01 == "") return;
                if (playerStats.currentStima <= weapons.baseStimaCost * weapons.heavyAttackStimaCoefficient) return;
                animatorController.PlayTargetAnimation(weapons.One_Hand_Heavy_Attack_01, true);
            }
        }

        /// <summary>
        /// 背刺
        /// </summary>
        public void HandleBackStab()
        {
            RaycastHit hit;

            //判断当前的位置和朝向是否可以背刺到目标
            if (Physics.Raycast(criticalAttackRaycastPoint.position,
                    transform.TransformDirection(Vector3.forward), out hit, 0.5f, backStabLayer))
            {
                if (hit.collider.gameObject.CompareTag("Back Stab") == false)
                {
                    return;
                }

                CharaManager enemyCharaManager = hit.transform.gameObject.GetComponentInParent<CharaManager>();
                DamageCollider rightWeapon = weaponSlotManager.rightHandDamageCollider; //获取当前武器的数值

                if (enemyCharaManager != null)
                {
                    //将角色定位到背刺的站位
                    playerManager.transform.position =
                        enemyCharaManager.backStabCollider.criticalDamagerStandPoint.position;

                    //调整角色的朝向
                    Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation =
                        Quaternion.Slerp(playerManager.transform.rotation, tr, 500f * Time.deltaTime);
                    playerManager.transform.rotation = targetRotation;

                    int criticalDamage = rightWeapon.weaponDamage *
                                         playerInventory.rightHandWeapon.criticalDamageMultiplier;
                    enemyCharaManager.pendingCriticalDamage = criticalDamage; //注意这里是被背刺的目标即将受到的伤害。

                    //播放背刺和被背刺的动画
                    animatorController.PlayTargetAnimation("Back Stab", true);
                    enemyCharaManager.GetComponentInChildren<AnimatorManager>()
                        .PlayTargetAnimation("Back Stabbed", true);
                }
            }
            //判断当前玩家和敌人的站位能否触发弹反处决
            else if (Physics.Raycast(criticalAttackRaycastPoint.position,
                         transform.TransformDirection(Vector3.forward), out hit, 0.75f, riposteLayer))
            {
                CharaManager enemyCharaManager = hit.transform.gameObject.GetComponentInParent<CharaManager>();
                DamageCollider rightWeapon = weaponSlotManager.rightHandDamageCollider;

                //如果敌人的当前状态可以被处决
                if (enemyCharaManager != null && enemyCharaManager.canBeRiposted)
                {
                    playerManager.transform.position =
                        enemyCharaManager.riposteCollider.criticalDamagerStandPoint.position;

                    Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation =
                        Quaternion.Slerp(playerManager.transform.rotation, tr, 500f * Time.deltaTime);
                    playerManager.transform.rotation = targetRotation;

                    int criticalDamage = rightWeapon.weaponDamage *
                                         playerInventory.rightHandWeapon.criticalDamageMultiplier;
                    enemyCharaManager.pendingCriticalDamage = criticalDamage;

                    animatorController.PlayTargetAnimation("Parry Stab", true);
                    enemyCharaManager.GetComponentInChildren<AnimatorManager>()
                        .PlayTargetAnimation("Parry Stabbed", true);
                }
            }
        }

        private void PerformWeaponArt(bool isLeftWeapon)
        {
            if (inputManager.twoHandsFlag)
            {
            }
            else if (isLeftWeapon)
            {
                animatorController.PlayTargetAnimation(playerInventory.leftHandWeapon.weapon_Art_Animaton, true);
            }
            else
            {
            }
        }

        #endregion

        #region Defence Actions

        public void PerformBlockAction()
        {
            if (playerManager.isInteracting)
                return;

            if (playerManager.isBlocking)
                return;

            animatorController.PlayTargetAnimation("Block", false, true);
            inputManager.blockFlag = true;
            equipmentManager.OpenBlockCollider();
        }

        #endregion

        private void ResetComboCount()
        {
            //不攻击几秒后中断当前连招。
            //但其实目前没有什么卵用，首先目前的连招触发机制并不允许从中间开始释放连击，而且我在第一下攻击中重置了连招计数器
            comboTimeBuffer -= Time.deltaTime;
            if (comboTimeBuffer <= 0)
            {
                comboCount = 0;
            }
        }
    }
}