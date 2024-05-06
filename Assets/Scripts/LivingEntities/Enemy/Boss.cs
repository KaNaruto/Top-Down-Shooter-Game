using UnityEngine;

namespace LivingEntities.Enemy
{
    public class Boss : global::LivingEntities.Enemy.Enemy
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private Color skinColor;
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            AttackDistanceThreshold = 1;
            SetCharacteristics(moveSpeed,1,startingHealth,skinColor);
        }

        // Update is called once per frame
        void Update()
        {
            CheckAttack();
        }
    }
}
