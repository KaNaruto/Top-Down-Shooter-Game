using LivingEntities.Enemy;
using UnityEngine;

namespace Potions
{
    public class PlayerVisibilityManager : MonoBehaviour
    {
        private static PlayerVisibilityManager _instance;
        public static PlayerVisibilityManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject gm = new GameObject("EffectManager");
                    _instance = gm.AddComponent<PlayerVisibilityManager>();
                    DontDestroyOnLoad(gm);
                }
                return _instance;
            }
        }

        public bool IsPlayerVisible { get; private set; } = true;

   

        public void SetPlayerVisible(bool visible)
        {
            IsPlayerVisible = visible;
            // Call the method to invoke the event
            Enemy.ChangePlayerVisibility(visible);
        }
    }
}
