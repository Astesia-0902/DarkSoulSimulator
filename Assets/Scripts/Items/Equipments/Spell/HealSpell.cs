using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    [CreateAssetMenu(menuName ="Spells/Healing Spell")]
    public class HealSpell : SpellItem_SO
    {
        public int healingAmount;

        public override void SpellCasting(PlayerAnimatorManager animatorController, PlayerStats charaStats, WeaponSlotManager weaponSlotManager)
        {
            base.SpellCasting(animatorController, charaStats, weaponSlotManager);
            GameObject instantiatedWarmUpFX = Instantiate(spellWarmUpFX, animatorController.transform.position, Quaternion.identity);
            animatorController.PlayTargetAnimation(spellAnimation, true);
        }

        public override void SpellCasted(PlayerAnimatorManager animatorController, PlayerStats charaStats)
        {
            base.SpellCasted(animatorController, charaStats);
            GameObject instantiatedCastedFX = Instantiate(spellCastFX, animatorController.transform.position, Quaternion.identity);
            charaStats.HealChara(healingAmount);
        }
    }
}