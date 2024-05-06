using System;
using System.Globalization;
using LivingEntities.Enemy;
using LivingEntities.Player;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Combat.Skills
{
    [Serializable]
    public class SkillUI : MonoBehaviour
    {
        [Header("Ability")] [SerializeField] protected Image icon;

        [SerializeField] protected TextMeshProUGUI cooldownText;
        [SerializeField] protected KeyCode key;
        [SerializeField] protected float cooldown;


        [FormerlySerializedAs("CurrentCooldown")] public float currentCooldown;
        protected bool IsInCooldown;

        // Start is called before the first frame update
        private void Awake()
        {
            FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
        }

    
        protected virtual void Start()
        {
            icon.fillAmount = 0;
            cooldownText.text = "";

            FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
        
        }


        protected virtual void Skill(KeyCode skillKeyCode, float skillCooldown)
        {
            if (Input.GetKeyDown(skillKeyCode) && !IsInCooldown)
            {
                IsInCooldown = true;
                currentCooldown = skillCooldown;
            }
        }

        protected virtual void SkillCooldown(Image cooldownIcon, TextMeshProUGUI text, float cooldownTime,
            ref float currentCooldownTime, ref bool isInCooldown)
        {
            if (isInCooldown)
            {
                currentCooldownTime -= Time.deltaTime;
                if (currentCooldownTime <= 0f)
                {
                    isInCooldown = false;
                    currentCooldownTime = 0f;
                    if (cooldownIcon != null)
                        cooldownIcon.fillAmount = 0f;
                    if (text != null)
                        text.text = "";
                }
                else
                {
                    if (cooldownIcon != null)
                        cooldownIcon.fillAmount = currentCooldownTime / cooldownTime;

                    if (text != null)
                        text.text = Mathf.Ceil(currentCooldownTime).ToString(CultureInfo.CurrentCulture);
                }
            }
        }

        void OnNewWave(int waveNumber)
        {
            IsInCooldown = false;

            icon.fillAmount = 0;
            cooldownText.text = "";
        }

        void OnPlayerDeath()
        {
            GameObject skillHolder = GameObject.FindGameObjectWithTag("Skill Holder");
            if (skillHolder != null)
            {
                skillHolder.SetActive(false);
            }
        
        }
    }
}