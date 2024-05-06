using System.Collections;
using Combat.Firearm;
using UnityEngine;

namespace Combat.Skills
{
    public class Shoot360 : SkillUI
    {
        [SerializeField] private float projectileAmount = 10;
        [SerializeField] float projectileSpeed = 10;
        [SerializeField] private Projectile projectile;
    
        private float _rotateAngleIncrement;
    
        protected override void Start()
        {
            base.Start();
        
            _rotateAngleIncrement = 360 / projectileAmount;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(key) && !IsInCooldown)
                StartCoroutine(UseSkill());
        
            Skill(key, cooldown);
            SkillCooldown(icon, cooldownText, cooldown, ref currentCooldown, ref IsInCooldown);
        }

    

        IEnumerator UseSkill()
        {
            float currentRotateAngle = 0;
            while (currentRotateAngle <= 360)
            {
                float radianAngle = currentRotateAngle * Mathf.Deg2Rad;
                var shootDirection =
                    new Vector3(Mathf.Cos(radianAngle), 0, Mathf.Sin(radianAngle));
                var shootPosition = transform.position + shootDirection;

                Projectile newProjectile = Instantiate(projectile, shootPosition, Quaternion.identity);
                // Rotate the projectile to face the direction it's moving
                newProjectile.transform.rotation = Quaternion.LookRotation(shootDirection);

                newProjectile.SetSpeed(projectileSpeed);
                currentRotateAngle += _rotateAngleIncrement;
                yield return null;
            }

            yield return new WaitForSeconds(cooldown);
        }
    }
}