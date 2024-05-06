using UnityEngine;

namespace Combat.Firearm
{
    public class Projectile : MonoBehaviour
    {
        public float Speed { get; private set; } = 10f;


        [SerializeField] float damage = 1;
        [SerializeField] LayerMask collisionMask;

        private readonly float _lifeTimeInSeconds = 3;
        private readonly float _skinWidth = .1f;

        [SerializeField] Color trailColor;

        public void SetSpeed(float newSpeed)
        {
            Speed = newSpeed;
        }

        public void SetSlowMotionProjectileSpeed(float newSpeed)
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject, _lifeTimeInSeconds);

            Collider[] initialCollision = Physics.OverlapSphere(transform.position, .1f, collisionMask);
            if (initialCollision.Length > 0)
                OnHitObject(initialCollision[0], transform.position);

            initialCollision = Physics.OverlapSphere(transform.position, .1f);
            if (initialCollision.Length > 0)
                OnHitObject(initialCollision[0], transform.position);

            GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
        }

        // Update is called once per frame
        void Update()
        {
            ProjectileMovement();
        }

        void ProjectileMovement()
        {
            float moveDistance = Speed * Time.deltaTime;
            transform.Translate(Vector3.forward * moveDistance);
            CheckCollisions(moveDistance);
        }

        void CheckCollisions(float moveDistance)
        {
            var transform1 = transform;
            Ray ray = new Ray(transform1.position, transform1.forward);

            if (Physics.Raycast(ray, out var hit, moveDistance + _skinWidth, collisionMask,
                    QueryTriggerInteraction.Collide))
            {
                OnHitObject(hit.collider, hit.point);
            }

            if (Physics.Raycast(ray, out var hitAnyObject, moveDistance + _skinWidth))
            {
                Destroy(gameObject);
            }
        }

        void OnHitObject(Collider c, Vector3 hitPoint)
        {
            IDamageable damageableObject = c.GetComponent<IDamageable>();
            if (damageableObject != null)
                damageableObject.TakeHit(damage, hitPoint, transform.forward);
            GameObject.Destroy(gameObject);
        }
    }
}