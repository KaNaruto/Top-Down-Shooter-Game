using UnityEngine;

namespace Potions
{
    public class EffectManager : MonoBehaviour
    {
        private static EffectManager _instance;
        public static EffectManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject gm = new GameObject("EffectManager");
                    _instance = gm.AddComponent<EffectManager>();
                    DontDestroyOnLoad(gm);
                }
                return _instance;
            }
        }
    }
}
