using System.Collections;
using LivingEntities;
using LivingEntities.Player;
using UnityEngine;

namespace Potions
{
    [CreateAssetMenu(menuName = "Powerups/Invisibility Potion")]
    public class InvisibilityPotion : PotionEffect
    {
        public float fadeDuration = 0.5f;
        public float duration = 5f;
        public float transparencyValue = 0.5f;

        private Color _originalColor;
        private Color _transparentColor;
        private Renderer _renderer;
        private float _elapsedTime;

        private LivingEntity _player;

        private void OnEnable()
        {
            effect = "Invisibility for " + duration + " seconds";
        }

        public override void Apply(GameObject target)
        {
            _renderer = target.GetComponent<Renderer>();
    
            if (_renderer == null)
            {
                return;
            }
        
            if (EffectManager.Instance != null && target != null)
            {
                EffectManager.Instance.StartCoroutine(ApplyI(target));
            }
       
        }

        public override IEnumerator ApplyI(GameObject target)
        {
            _player = FindObjectOfType<Player>().GetComponent<LivingEntity>();
        
            // Initialize fade effect
            _elapsedTime = 0;
            float t = 0;
            _originalColor = _renderer.material.color;
            _transparentColor = new Color(_originalColor.r, _originalColor.g, _originalColor.b, transparencyValue);

            // Fade to transparent
            while (t < 1)
            {
                t = Mathf.Clamp01(_elapsedTime / fadeDuration);
                _renderer.material.color = Color.Lerp(_originalColor, _transparentColor, t);
                _elapsedTime += Time.deltaTime;
                yield return null;
            }
            _renderer.material.color = _transparentColor;
            _player.IsVisible = false;
            PlayerVisibilityManager.Instance.SetPlayerVisible(false);
        
            // Maintain invisibility
            yield return new WaitForSeconds(duration);

            // Fade back to original color
            _elapsedTime = 0;
            t = 0;
            while (t < 1)
            {
                t = Mathf.Clamp01(_elapsedTime / fadeDuration);
                _renderer.material.color = Color.Lerp(_transparentColor, _originalColor, t);
                _elapsedTime += Time.deltaTime;
                yield return null;
            }
            _renderer.material.color = new Color(_originalColor.r,_originalColor.g,_originalColor.b,1);
            _player.IsVisible = true;
            PlayerVisibilityManager.Instance.SetPlayerVisible(true);
        }
    }
}
