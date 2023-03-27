using Astesia;
using UnityEngine;

namespace Items.Equipments.Spell
{
    [CreateAssetMenu (menuName = "Spells/Projectile Spell")]
    public class ProjectileSpell : SpellItem_SO
    {
        public float baseDamage;
        public float projectileSpeed;
        private Rigidbody rb;

        public override void SpellCasting(PlayerAnimatorManager animatorController,
            PlayerStats charaStats,
            WeaponSlotManager weaponSlotManager)

        {
            base.SpellCasting(animatorController, charaStats, weaponSlotManager);
            //Instantiate the spell cast FX on the player's hand
            GameObject instantiatedWarmUpFX = Instantiate(spellWarmUpFX,
                weaponSlotManager.rightHandHolderSlot.transform.position,
                Quaternion.identity);
            //set the scale of the spell cast FX to 1
            instantiatedWarmUpFX.transform.localScale = new Vector3(1, 1, 1);
            //play the spell animation
            animatorController.PlayTargetAnimation(spellAnimation, true);
        }

        public override void SpellCasted(PlayerAnimatorManager animatorController,
            PlayerStats charaStats)
        {
            base.SpellCasted(animatorController, charaStats);
        }
    }
}