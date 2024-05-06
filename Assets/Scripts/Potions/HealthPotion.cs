using System.Collections;
using LivingEntities.Player;
using UnityEngine;

namespace Potions
{
    [CreateAssetMenu(menuName = "Powerups/Health Potion")]
    internal class HealthPotion : PotionEffect
    {
        public float amount;

        private void OnEnable()
        {
            effect = "Health +" + amount;
        }

        public override void Apply(GameObject target)
        {
            target.GetComponent<Player>().Health += amount;
        }

        public override IEnumerator ApplyI(GameObject target)
        {
            yield break;
        }
    }
}