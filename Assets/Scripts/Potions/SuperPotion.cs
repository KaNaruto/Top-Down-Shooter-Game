using System.Collections;
using UnityEngine;

namespace Potions
{
    [CreateAssetMenu(menuName = "Powerups/Super Potion")]
    public class SuperPotion : PotionEffect
    {

        [SerializeField] private PotionEffect[] potionEffects;
        [SerializeField] private float effectTime;
        private readonly float _flashSpeed = 6;

        private void OnEnable()
        {
            effect = "SUPER POTION!!!";
        }

        public override void Apply(GameObject target)
        {
            foreach (var potion in potionEffects)
            {
                potion.Apply(target);
            }

            EffectManager.Instance.StartCoroutine(ApplyI(target));
        }

        public override IEnumerator ApplyI(GameObject target)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            Color originalColor = renderer.material.color;
            Gradient colorGradient = new Gradient();
            // Define your gradient here
            colorGradient.SetKeys(
                new[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.yellow, 0.5f), new GradientColorKey(Color.red, 1.0f) },
                new[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(1.0f, 1.0f) }
            );

            float elapsedTime = 0;
            while (elapsedTime < effectTime)
            {
                float t = Mathf.PingPong(elapsedTime * _flashSpeed, 1);
                renderer.material.color = colorGradient.Evaluate(t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            renderer.material.color = originalColor;
        }
    }
}
