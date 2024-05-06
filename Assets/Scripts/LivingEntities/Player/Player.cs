using Audio;
using Combat.Firearm;
using LivingEntities.Enemy;
using UnityEngine;

namespace LivingEntities.Player
{
    [RequireComponent(typeof(PlayerController), typeof(GunController))]
    public class Player : LivingEntity
    {
        private PlayerController _playerController;
        [SerializeField] Camera viewCamera;
        private GunController _gunController;

        [SerializeField] Crosshair crosshair;
    
        // Start is called before the first frame update

        void OnNewWave(int waveNumber)
        {
            if(Health>(startingHealth/4))
            {
                Health = startingHealth;
            }
            else
            {
                Health += startingHealth / 4;
            }
            _gunController.EquipGun((waveNumber-1)%5);
        }

        protected override void Start()
        {
            base.Start();

            _playerController = GetComponent<PlayerController>();
            viewCamera = Camera.main;
        }

        private void Awake()
        {
            _gunController = GetComponent<GunController>();
            FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
        }


        // Update is called once per frame
        void Update()
        {
            MovementInput();
            RotateInput();
            GunFireInput();

            // Reload
            if (Input.GetKeyDown(KeyCode.R))
                _gunController.Reload();
        
            // if player falls from the map
            if(transform.position.y<-10)
                TakeDamage(Health);
        }

        void MovementInput()
        {
            Vector3 playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            Vector3 dir = playerInput.normalized;
            _playerController.Move(dir);
        }

        void RotateInput()
        {
            Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.up * _gunController.GetGunHeight);

            if (groundPlane.Raycast(ray, out var rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                _playerController.RotateTo(point);

                //Crosshair
                crosshair.transform.position = point;
                crosshair.DetectTargets(ray);

                // Gun aim to crosshair
                if ((new Vector2(point.x, point.y) - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude >
                    1)
                    _gunController.Aim(point);
            }
        }

        void GunFireInput()
        {
            if (Input.GetMouseButton(0))
                _gunController.OnTriggerHold();
            if (Input.GetMouseButtonUp(0))
                _gunController.OnTriggerReleased();
        }

        public override void Die()
        {
            AudioManager.Instance.PlaySound("Player Death",transform.position);
            base.Die();
        }
    }
}