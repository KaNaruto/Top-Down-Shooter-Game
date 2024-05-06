using System.Collections;
using UnityEngine;

namespace Potions
{
    public abstract class PotionEffect : ScriptableObject
    {

        public string effect;
        public abstract void Apply(GameObject target);

        public abstract IEnumerator ApplyI(GameObject target);
        
    }
}
