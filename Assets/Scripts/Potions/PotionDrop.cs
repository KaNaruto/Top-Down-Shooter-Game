using LivingEntities.Enemy;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Potions
{
    public class PotionDrop : MonoBehaviour
    {
        [Range(0, 1)] [SerializeField] private float dropRate; // The chance of a potion being dropped.
        [SerializeField] private GameObject[] potions; // The array of potion prefabs.
        [Range(0, 1)] [SerializeField] private float[] potionsDropRate; // The drop rate for each potion.
        [SerializeField] private Enemy enemy;


        private void Awake()
        {
            if (enemy != null)
                enemy.OnDeath += OnEnemyDeath; // Subscribe to the enemy's death event.
        }

        private void OnEnemyDeath()
        {
            Random random = new Random((uint)UnityEngine.Random.Range(0, int.MaxValue));
            float randomNumber = random.NextFloat();

            if (randomNumber <= dropRate)
            {
                float totalRate = 0f;
                foreach (float rate in potionsDropRate)
                {
                    totalRate += rate; // Sum up the total drop rate.
                }

                for (int i = 0; i < potionsDropRate.Length; i++)
                {
                    potionsDropRate[i] /= totalRate; // Normalize the drop rates.
                }

                randomNumber = random.NextFloat(); // Random number for selecting which potion to drop.

                float floor = 0;
                for (int i = 0; i < potions.Length; i++)
                {
                    float ceil = floor; // Lower bound of the current potion's drop range.
                    floor += potionsDropRate[i]; // Upper bound of the current potion's drop range.

                    if (randomNumber >= ceil && randomNumber < floor)
                    {
                        Instantiate(potions[i], transform.position, Quaternion.identity); // Spawn the potion.
                        break; // Ensure only one potion is dropped.
                    }
                }
            }
        }
    }
}