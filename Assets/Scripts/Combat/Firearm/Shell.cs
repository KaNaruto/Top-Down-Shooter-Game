using System.Collections;
using UnityEngine;

namespace Combat.Firearm
{
    public class Shell : MonoBehaviour
    {
        [SerializeField] Rigidbody myRigidBody;

        [SerializeField] float forceMin;
        [SerializeField] float forceMax;

        private readonly float _lifeTime = 4; // In Seconds
        private readonly float _fadeTime = 2; // In Seconds

        // Start is called before the first frame update
        void Start()
        {
            float force = Random.Range(forceMin, forceMax);
            myRigidBody.AddForce(transform.right * force);
            myRigidBody.AddTorque(Random.insideUnitSphere*force);

            StartCoroutine(Fade());
        }

        IEnumerator Fade()
        {

            yield return new WaitForSeconds(_lifeTime);

            float fadePercent = 0;
            float fadeSpeed = 1 / _fadeTime;
            Material mat = GetComponent<Renderer>().material;
            Color initialColor = mat.color;
            while (fadePercent<1)
            {
                fadePercent += Time.deltaTime * fadeSpeed;
                mat.color = Color.Lerp(initialColor, Color.clear, fadePercent);
                yield return null;
            }
        
            Destroy(gameObject);
        }
    }
}