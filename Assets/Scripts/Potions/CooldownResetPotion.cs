using System.Collections;
using Combat.Skills;
using UnityEngine;

namespace Potions
{
    [CreateAssetMenu(menuName = "Powerups/Cooldown Reset Potion")]
    public class CooldownResetPotion : PotionEffect
    {
        private void OnEnable()
        {
            effect = "Cooldown Reset";
        }

        public override void Apply(GameObject target)
        {
            SkillUI[] skills = target.GetComponentsInChildren<SkillUI>(true);
            foreach (var skill in skills)
            {
                skill.currentCooldown = 0.01f;
            }
        }

        public override IEnumerator ApplyI(GameObject target)
        {
            yield return null;
        }
    }
}
