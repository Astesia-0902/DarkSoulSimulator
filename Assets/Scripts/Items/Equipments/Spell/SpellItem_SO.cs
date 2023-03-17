using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public enum SpellType
    {
        FaithSpell,MagicSpell,PyroSpell
    }

    public class SpellItem_SO : Item_SO
    {
        public GameObject spellWarmUpFX;
        public GameObject spellCastFX;
        public string spellAnimation;

        public int manaCost;

        [Header("Spell Type")]
        public SpellType spellType;

        [Header("Spell Description")]
        [TextArea]
        public string spellDescription;

        public virtual void SpellCasting(PlayerAnimatorManager animatorController, PlayerStats charaStats)
        {

        }

        public virtual void SpellCasted(PlayerAnimatorManager animatorController, PlayerStats charaStats)
        {
            charaStats.DrainMana(manaCost);
        }
    }
}
