using UnityEngine;
using Random = UnityEngine.Random;

namespace Combat.Firearm
{
    public class MuzzleFlash : MonoBehaviour
    {
        [SerializeField] GameObject muzzleFlashHolder;
        [SerializeField] float flashTime;
        [SerializeField] Sprite[] flashSprites;
        [SerializeField] SpriteRenderer[] spriteRenderers;

        private void Start()
        {
            Deactivate();
        }

        public void Activate()
        {
            muzzleFlashHolder.SetActive(true);
            int flashSpriteIndex = Random.Range(0, flashSprites.Length);
            foreach (var t in spriteRenderers)
            {
                t.sprite = flashSprites[flashSpriteIndex];
            }

            Invoke(nameof(Deactivate), flashTime);
        }

        void Deactivate()
        {
            muzzleFlashHolder.SetActive(false);
        }
    }
}