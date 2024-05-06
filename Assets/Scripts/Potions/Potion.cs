using System;
using System.Collections;
using LivingEntities.Enemy;
using UnityEngine;

namespace Potions
{
    public class Potion : MonoBehaviour
    {
        [SerializeField] PotionEffect potionEffect;
        private readonly float _offsetY = 0.5f;
        private readonly float _angle = 1;
        private readonly float _speed = 0.5f;
   
        private GameObject _target;
        private Spawner _spawner;

        public static event Action<string> OnPotionUse;

        private void Awake()
        {
            _spawner = FindObjectOfType<Spawner>();
        }

        private void Start()
        {
            StartCoroutine(nameof(FloatAnimation));
        }

        private void OnEnable()
        {
            _spawner.OnNewWave += OnNewWave;
        }

        private void OnDisable()
        {
            _spawner.OnNewWave -= OnNewWave;
        }

        private void OnNewWave(int wave)
        {
            if(wave>1)
            {
                Potion[] potions = FindObjectsOfType<Potion>();
                foreach (Potion potion in potions)
                {
                    Destroy(potion.gameObject);
                }
            }
        }

        private IEnumerator FloatAnimation()
        {
            var position = transform.position;
            float ceilY = position.y + _offsetY;
            float floorY = position.y - _offsetY;
            while (true)
            {
                while (transform.position.y <= ceilY)
                {
                    transform.Translate(Vector3.up * (_offsetY / _speed * Time.deltaTime));
                    (transform).Rotate(Vector3.up, _angle);
                    yield return null;
                }
                while (transform.position.y >= floorY)
                {
                    transform.Translate(Vector3.down * (_offsetY / _speed * Time.deltaTime));
                    transform.Rotate(Vector3.up, _angle);
                    yield return null;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                OnPotionUse?.Invoke(potionEffect.effect);
                Destroy(gameObject);
                _target = other.gameObject;
                potionEffect.Apply(_target);
            }
        }
    }
}
