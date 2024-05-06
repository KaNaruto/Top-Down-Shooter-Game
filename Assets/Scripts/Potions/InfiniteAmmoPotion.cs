using System;
using System.Collections;
using Combat.Firearm;
using UnityEngine;

namespace Potions
{
    [CreateAssetMenu(menuName = "Powerups/Infinite Ammo Potion")]
    public class InfiniteAmmoPotion : PotionEffect
    {
        public float effectTime;

        private void OnEnable()
        {
            effect = "Infinite ammo for " + effectTime + " seconds";
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
        
            if (target != null && FindObjectOfType<GunController>().EquippedGun != null)
            {
                Gun gunComponent = FindObjectOfType<GunController>().EquippedGun;
                int originalAmmo = gunComponent.projectileRemainingInMag;
                gunComponent.projectileRemainingInMag = Int32.MaxValue;

                yield return new WaitForSeconds(effectTime);

                // Restore original ammo if Gun component still exists
                if (gunComponent != null)
                {
                    gunComponent.projectileRemainingInMag = originalAmmo;
                }
            }
            else
            {
                Debug.LogError("Target or Gun component is null.");
            }
        }
    }
}