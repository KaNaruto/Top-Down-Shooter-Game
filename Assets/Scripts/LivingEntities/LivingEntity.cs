using System;
using UnityEngine;

namespace LivingEntities
{
    public class LivingEntity : MonoBehaviour, IDamageable
    {
        public float startingHealth;
        public float Health { get; set; }
        protected bool Dead;

        public bool IsVisible { get; set; } = true;
        public event Action OnDeath;

        protected virtual void Start()
        {
            Health = startingHealth;
        }

        public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
        {
            TakeDamage(damage);
        }

        public virtual void TakeDamage(float damage)
        {
            Health -= damage;
            if (Health <= 0 && !Dead)
                Die();
        }

        [ContextMenu("Self Destruct")]
        public virtual void Die()
        {
            Dead = true;
            if (OnDeath != null)
                OnDeath();
            GameObject.Destroy(gameObject);
        }
    }
}