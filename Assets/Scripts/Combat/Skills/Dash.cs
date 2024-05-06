using System;
using System.Collections;
using LivingEntities.Player;
using UnityEngine;

namespace Combat.Skills
{
    public class Dash : SkillUI
    {
        [Header("Dash Settings")] [SerializeField]
        float dashSpeed = 20f;

        [SerializeField] float duration = .1f;
        [SerializeField] private TrailRenderer tr;
    
        private Vector3 _direction;
    
        public static event Action OnDashStart;
        public static event Action OnDashEnd;

    

        // Update is called once per frame
        void Update()
        {
            Vector3 playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            _direction = playerInput.normalized;
            if (Input.GetKeyDown(key) && !IsInCooldown)
            {
                StartCoroutine(nameof(UseDash));
            }
            Skill(key,cooldown);
            SkillCooldown(icon,cooldownText,cooldown,ref currentCooldown,ref IsInCooldown);
        }

        private IEnumerator UseDash()
        {
            OnDashStart?.Invoke();
            PlayerController playerController = FindObjectOfType<PlayerController>();
            Rigidbody playerRigidbody = playerController.GetComponent<Rigidbody>();
            tr.emitting = true;
        
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                playerRigidbody.velocity = _direction * dashSpeed;
                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
            playerRigidbody.velocity = Vector3.zero;
            tr.emitting = false;
            OnDashEnd?.Invoke();
        }
    }
}