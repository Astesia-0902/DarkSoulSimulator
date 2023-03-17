using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Astesia
{
    public class DamagePlayer : MonoBehaviour
    {
        int damage = 30;

        private void OnCollisionEnter(Collision collision)
        {
            var playerStats = collision.gameObject.GetComponent<PlayerStats>();
            playerStats.TakeDamage(damage);
        }
    }
}
