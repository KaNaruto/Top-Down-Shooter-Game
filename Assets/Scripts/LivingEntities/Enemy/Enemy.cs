using System;
using System.Collections;
using Audio;
using Combat.Skills;
using UnityEngine;
using UnityEngine.AI;

namespace LivingEntities.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : LivingEntity
    {
        private enum State { Idle, Chasing, Attacking }

        [SerializeField] ParticleSystem deathEffect;
        private State _currentState;
        private NavMeshAgent _pathFinder;
        private Transform _target;
        private bool _hasTarget;
        private LivingEntity _targetEntity;
        [SerializeField] float refreshRate = .25f;
        protected  float AttackDistanceThreshold = .5f;
        private readonly float _timeBetweenAttacks = 1f;
        private float _nextAttackTime;
        [SerializeField] float damage = 1;
        private Material _skinMaterial;
        private Color _originalColor;
        private float _myCollisionRadius;
        private float _targetCollisionRadius;
        public static event Action OnDeathStatic;
        public static event Action<bool> OnPlayerVisibilityChanged;

        private void Awake()
        {
            // Initialize pathfinder and collision radii
            _pathFinder = GetComponent<NavMeshAgent>();
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                _target = GameObject.FindGameObjectWithTag("Player").transform;
                _myCollisionRadius = GetComponent<CapsuleCollider>().radius;
                _targetCollisionRadius = _target.GetComponent<CapsuleCollider>().radius;
                _targetEntity = _target.GetComponent<LivingEntity>();
                _hasTarget = _targetEntity.IsVisible;
            }
        }
    
        protected void OnEnable()
        {
            // Subscribe to events and adjust death effect speed
            var module = deathEffect.main;
            module.simulationSpeed = 1;
            OnPlayerVisibilityChanged += HandlePlayerVisibilityChanged;
            SlowMotion.OnSlowMotionStart += ApplySlowMotion;
            SlowMotion.OnSlowMotionEnd += RemoveSlowMotion;
        }

        protected void OnDisable()
        {
            // Unsubscribe from events
            OnPlayerVisibilityChanged -= HandlePlayerVisibilityChanged;
            SlowMotion.OnSlowMotionStart -= ApplySlowMotion;
            SlowMotion.OnSlowMotionEnd -= RemoveSlowMotion;
        }

        protected override void Start()
        {
            base.Start();
            if (_hasTarget)
            {
                _currentState = State.Chasing;
                _targetEntity.OnDeath += OnTargetDeath;
                StartCoroutine(UpdatePath());
            }
        }

        public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
        {
            // Play impact sound and handle death
            AudioManager.Instance.PlaySound("Impact", transform.position);
            if (damage >= Health)
            {
                OnDeathStatic?.Invoke();
                AudioManager.Instance.PlaySound("Enemy Death", transform.position);
                Destroy(Instantiate(deathEffect, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)), deathEffect.main.startLifetimeMultiplier);
            }
            base.TakeHit(damage, hitPoint, hitDirection);
        }

        void OnTargetDeath()
        {
            // Handle target death
            _hasTarget = false;
            _currentState = State.Idle;
        }

        public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, float enemyHealth, Color skinColor)
        {
            // Set enemy characteristics
            _pathFinder.speed = moveSpeed;
            if (_hasTarget)
            {
                damage = Mathf.Ceil(_targetEntity.startingHealth / hitsToKillPlayer);
            }
            startingHealth = enemyHealth;
            var particleSystemMainModule = deathEffect.main;
            particleSystemMainModule.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1);
            _skinMaterial = GetComponent<Renderer>().material;
            _skinMaterial.color = skinColor;
            _originalColor = _skinMaterial.color;
        }

        private void Update()
        {
            CheckAttack();
        }

        protected void CheckAttack()
        {
            // Handle attack timing and targeting
            if (_hasTarget && Time.time >= _nextAttackTime)
            {
                float sqrtDistanceToTarget = (_target.position - transform.position).sqrMagnitude;
                if (sqrtDistanceToTarget < MathF.Pow(AttackDistanceThreshold + _myCollisionRadius + _targetCollisionRadius, 2))
                {
                    _nextAttackTime = Time.time + _timeBetweenAttacks;
                    AudioManager.Instance.PlaySound("Enemy Attack", transform.position);
                    StartCoroutine(Attack());
                }
            }
        }

        IEnumerator Attack()
        {
            // Attack sequence
            if (_hasTarget)
            {
                _currentState = State.Attacking;
                _pathFinder.enabled = false;
                var position = transform.position;
                Vector3 originalPosition = position;
                var position1 = _target.position;
                Vector3 dirToTarget = (position1 - position).normalized;
                Vector3 attackPosition = position1 - dirToTarget * (_myCollisionRadius);
                float attackSpeed = 3;
                float percent = 0;
                bool hasAppliedDamage = false;
                _skinMaterial.color = Color.red;
                while (percent <= 1)
                {
                    if (percent >= .5f && !hasAppliedDamage)
                    {
                        hasAppliedDamage = true;
                        _targetEntity.TakeDamage(damage);
                    }
                    percent += Time.deltaTime * attackSpeed;
                    float interpolation = (-MathF.Pow(percent, 2) + percent) * 4;
                    transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
                    yield return null;
                }
                _skinMaterial.color = _originalColor;
                _currentState = State.Chasing;
                _pathFinder.enabled = true;
            }
        }

        IEnumerator UpdatePath()
        {
            // Path update sequence
            while (_hasTarget)
            {
                if (_currentState == State.Chasing)
                {
                    var position = _target.position;
                    Vector3 dirToTarget = (position - transform.position).normalized;
                    Vector3 targetPosition = position - dirToTarget * (_myCollisionRadius + _targetCollisionRadius + AttackDistanceThreshold / 2);
                    if (!Dead)
                        _pathFinder.SetDestination(targetPosition);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }

        private void ApplySlowMotion(float slowRate)
        {
            // Apply slow motion effect
            var module = deathEffect.main;
            module.simulationSpeed /= 3;
        }

        private void RemoveSlowMotion(float slowRate)
        {
            // Remove slow motion effect
            var module = deathEffect.main;
            module.simulationSpeed *= 3;
        }
    
        public static void ChangePlayerVisibility(bool isVisible)
        {
            // Change player visibility
            OnPlayerVisibilityChanged?.Invoke(isVisible);
        }
        private void HandlePlayerVisibilityChanged(bool isVisible)
        {
            // Handle player visibility change
            _hasTarget = isVisible;
            if (!isVisible)
            {
                _currentState = State.Idle;
                _pathFinder.enabled = false;
            }
            else
            {
                _currentState = State.Chasing;
                _pathFinder.enabled = true;
                StartCoroutine(nameof(UpdatePath));
            }
        }
    }
}
