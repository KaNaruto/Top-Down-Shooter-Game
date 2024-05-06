using System;
using System.Collections;
using Combat.Firearm;
using LivingEntities.Enemy;
using UnityEngine;
using UnityEngine.AI;

namespace Combat.Skills
{
    public class SlowMotion : SkillUI
    {
        [SerializeField] private float duration;
        [Range(0, 1)] [SerializeField] private float slowMotionRate;

        private Enemy[] _enemies;
        private Projectile[] _projectiles;

        public static event Action<float> OnSlowMotionStart;
        public static event Action<float> OnSlowMotionEnd;

        private float _startTime;

        protected override void Start()
        {
            base.Start();
            slowMotionRate = 1 - slowMotionRate;
        }

        void Update()
        {
            if (Input.GetKeyDown(key) && !IsInCooldown)
                if (!Spawner.IsBossAlive)
                    StartCoroutine(SlowDown());

            if (Time.time - _startTime >= duration)
            {
                SkillCooldown(icon, cooldownText, cooldown, ref currentCooldown, ref IsInCooldown);
            }
        }


        IEnumerator SlowDown()
        {
            Skill(key, cooldown);
            _startTime = Time.time;


            OnSlowMotionStart?.Invoke(slowMotionRate);
            // Slow down enemies and projectiles
            Spawner spawner = FindObjectOfType<Spawner>();
            if (spawner == null) yield break;

            Spawner.Wave currentWave = spawner.currentWave;
            float originalEnemySpeed = currentWave.moveSpeed;

            // Slow down enemy spawning and visual effects
            spawner.spawnDelay *= 3;
            spawner.tileFlashSpeed /= 3;
            currentWave.moveSpeed *= slowMotionRate;

            _enemies = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in _enemies)
            {
                NavMeshAgent enemyAgent = enemy.GetComponent<NavMeshAgent>();
                if (enemyAgent != null && !enemyAgent.isStopped)
                {
                    enemyAgent.speed *= slowMotionRate;
                }
            }

            // Slow down already fired bullets
            _projectiles = FindObjectsOfType<Projectile>();
            float originalProjectileSpeed = _projectiles.Length > 0 ? _projectiles[0].Speed : 10;
            foreach (Projectile proj in _projectiles)
            {
                if (proj != null)
                    proj.SetSpeed(originalProjectileSpeed * slowMotionRate);
            }

            // Slow down gun muzzle velocity
            yield return new WaitForSeconds(duration);

            // Restore original speeds
            _enemies = FindObjectsOfType<Enemy>();
            foreach (Enemy enemy in _enemies)
            {
                if (enemy != null)
                {
                    NavMeshAgent enemyAgent = enemy.GetComponent<NavMeshAgent>();
                    if (enemyAgent != null && !enemyAgent.isStopped)
                    {
                        enemyAgent.speed = originalEnemySpeed;
                    }
                }
            }

            currentWave.moveSpeed = originalEnemySpeed;
            spawner.spawnDelay /= 3;
            spawner.tileFlashSpeed *= 3;

            _projectiles = FindObjectsOfType<Projectile>();
            foreach (Projectile proj in _projectiles)
            {
                if (proj != null)
                    proj.SetSpeed(originalProjectileSpeed);
            }

            OnSlowMotionEnd?.Invoke(slowMotionRate);
        }
    }
}