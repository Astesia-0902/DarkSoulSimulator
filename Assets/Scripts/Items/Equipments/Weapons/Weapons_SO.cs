using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astesia
{
    public enum WeaponType
    {
        FaithCaster, MagicCaster, PyroCaster, MeleeWeapon, Shield
    }

    [CreateAssetMenu(menuName ="Item/Weapon Item")]
    public class Weapons_SO : Item_SO
    {
        public GameObject itemPrefeb;
        public bool isUnarmed;

        [Header("Damage")]
        public int baseDamage = 25;
        public int criticalDamageMultiplier = 4;

        [Header("Absorption")] public float physicalDamageAbsorption;

        [Header("Weapon Idle Animations")]
        public string leftHandIdle;
        public string rightHandIdle;
        public string twoHandedIdle;

        [Header("One Hand Attack Animations")]
        public string[] attackNames = new string[10];
        public string One_Hand_Heavy_Attack_01;

        [Header("Two-Hand Attack Animations")]
        public string[] attackNames_TwoHanded = new string[10];

        [Header("Weapon Art Animations")]
        public string weapon_Art_Animaton;

        [Header("Stima Stats")]
        public int baseStimaCost;
        public float lightAttackStimaCoefficient;
        public float heavyAttackStimaCoefficient;
        public float twoHandAttackStimaCoefficient;

        [Header("Weapon Type")]
        public WeaponType weaponType;
    }
}
