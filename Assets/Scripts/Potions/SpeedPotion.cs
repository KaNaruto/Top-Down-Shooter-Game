using System.Collections;
using LivingEntities.Player;
using UnityEngine;

namespace Potions
{
    [CreateAssetMenu(menuName = "Powerups/Speed Potion")]
    public class SpeedPotion : PotionEffect
    {
        public float amount;
        public float effectTime;

        private void OnEnable()
        {
            effect = "Increased speed for " + effectTime + " seconds";
        }

        public override void Apply(GameObject target)
        {
            if (EffectManager.Instance != null)
            {
                EffectManager.Instance.StartCoroutine(ApplyI(target));
            }
            else
            {
                Debug.LogError("EffectManager instance is null.");
            }
        }

        public override IEnumerator ApplyI(GameObject target)
        {
        
            target.GetComponent<PlayerController>().moveSpeed += amount;

            yield return new WaitForSeconds(effectTime);

       
            target.GetComponent<PlayerController>().moveSpeed -= amount;
        }
    }
}