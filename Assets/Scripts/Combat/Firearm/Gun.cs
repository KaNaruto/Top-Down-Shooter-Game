using System;
using System.Collections;
using Audio;
using Combat.Skills;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Combat.Firearm
{
    public class Gun : MonoBehaviour
    {
        // Weapon variations
        private enum FireMode
        {
            Auto,
            Burst,
            Single
        }

        [SerializeField] FireMode fireMode;
        private bool _triggerReleasedSinceLastShot;

        // Burst
        [SerializeField] int burstCount;
        private int _shotsRemainingInBurst;


        [Header("Effects")] [SerializeField] Transform[] muzzles;
        [SerializeField] Transform shell;
        [SerializeField] Transform shellEjector;
        [SerializeField] AudioClip shootAudio;
        [SerializeField] AudioClip reloadAudio;


        private MuzzleFlash _muzzleFlash;

        // Reload
        [Header("Gun specs")] [SerializeField] float reloadTime = .3f;
        public int projectilePerMag;

    
    
        [SerializeField] Projectile projectile;
        [SerializeField] float msBetweenShoots = 100;
        [SerializeField] private float muzzleVelocity = 35;


        public  int projectileRemainingInMag;
    
        private float _nextShootTime;
        private bool _isReloading;

        // Recoil
        [Header("Recoil")] private Vector3 _recoilSmoothDampVelocity;
        private float _recoilRotSmoothDampVelocity;
        private float _recoilAngle;
        [SerializeField] Vector2 kickMinMax = new Vector2(.05f, .2f);
        [SerializeField] Vector2 recoilAngleMinMax = new Vector2(3, 5);
        [SerializeField] float recoilMoveSettleTime = .1f;
        [SerializeField] float recoilRotSettleTime = .1f;

        private void Start()
        {
            _muzzleFlash = GetComponent<MuzzleFlash>();
            _shotsRemainingInBurst = burstCount;
            projectileRemainingInMag = projectilePerMag;
        }

        protected void OnEnable()
        {
            SlowMotion.OnSlowMotionStart += ApplySlowMotion;
            SlowMotion.OnSlowMotionEnd += RemoveSlowMotion;
        }

        protected void OnDisable()
        {
            SlowMotion.OnSlowMotionStart -= ApplySlowMotion;
            SlowMotion.OnSlowMotionEnd -= RemoveSlowMotion;
        }

        void Shoot()
        {
            if (Time.time >= _nextShootTime && projectileRemainingInMag > 0 && !_isReloading)
            {
                // Burst shot
                if (fireMode == FireMode.Burst)
                {
                    if (_shotsRemainingInBurst == 0)
                        return;
                    _shotsRemainingInBurst--;
                }
                else if (fireMode == FireMode.Single) // Single shot
                    if (!_triggerReleasedSinceLastShot)
                    {
                        return;
                    }

                // Muzzle flash
                foreach (var muzzle in muzzles)
                {
                    if (projectileRemainingInMag == 0)
                        break;
                    projectileRemainingInMag--;
                    _nextShootTime = Time.time + (msBetweenShoots / 1000);
                    Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
                    newProjectile.SetSpeed(muzzleVelocity);
                }

                Instantiate(shell, shellEjector.position, shellEjector.rotation);
                _muzzleFlash.Activate();

                // Recoil
                transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
                _recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
                _recoilAngle = Mathf.Clamp(_recoilAngle, 0, 30);

                AudioManager.Instance.PlaySound(shootAudio, transform.position);
            }
        }

        private void Update()
        {
            transform.localEulerAngles = Vector3.left * _recoilAngle;
        }

        private void LateUpdate()
        {
            Recoil();
            if (!_isReloading && projectileRemainingInMag == 0)
                Reload();
        }

        public void Reload()
        {
            if (!_isReloading && projectileRemainingInMag != projectilePerMag)
            {
                StartCoroutine(AnimateReload());
                AudioManager.Instance.PlaySound(reloadAudio, transform.position);
            }
        }

        IEnumerator AnimateReload()
        {
            _isReloading = true;
            yield return new WaitForSeconds(0.2f);

            float reloadSpeed = 1 / reloadTime;
            float percent = 0;
            Vector3 initialRot = transform.localEulerAngles;
            float maxReloadAngle = 70;
            while (percent < 1)
            {
                percent += Time.deltaTime * reloadSpeed;
                float interpolation = (-MathF.Pow(percent, 2) + percent) * 4;
                float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
                transform.localEulerAngles = Vector3.left * reloadAngle + initialRot;

                yield return null;
            }

            _isReloading = false;
            projectileRemainingInMag = projectilePerMag;
        }


        // Recoil animation
        void Recoil()
        {
            transform.localPosition =
                Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref _recoilSmoothDampVelocity,
                    recoilMoveSettleTime);
            _recoilAngle = Mathf.SmoothDamp(_recoilAngle, 0, ref _recoilRotSmoothDampVelocity, recoilRotSettleTime);
            var transform1 = transform;
            transform1.localEulerAngles = transform1.localEulerAngles + Vector3.left * _recoilAngle;
        }


        private void ApplySlowMotion(float slowMotionRate)
        {
            muzzleVelocity *= slowMotionRate;
        }

        private void RemoveSlowMotion(float slowMotionRate)
        {
            muzzleVelocity /= slowMotionRate;
        }

        public void OnTriggerHold()
        {
            Shoot();
            _triggerReleasedSinceLastShot = false;
        }

        public void OnTriggerReleased()
        {
            _triggerReleasedSinceLastShot = true;
            _shotsRemainingInBurst = burstCount;
        }

        public void Aim(Vector3 aimPoint)
        {
            transform.LookAt(aimPoint);
        }
    }
}