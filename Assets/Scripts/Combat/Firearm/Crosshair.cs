using UnityEngine;

namespace Combat.Firearm
{
    public class Crosshair : MonoBehaviour
    {
        [SerializeField] LayerMask targetMask;
        [SerializeField] Color crosshairHighlightColor;
        private Color _originalCrosshairColor;

        private void Start()
        {
            Cursor.visible = false;
            _originalCrosshairColor = GetComponent<SpriteRenderer>().color;
        }

        private void Update()
        {
            transform.Rotate(Vector3.forward * (-40 * Time.deltaTime));
        }

        public void DetectTargets(Ray ray)
        {
            GetComponent<SpriteRenderer>().color = Physics.Raycast(ray,100,targetMask) ? crosshairHighlightColor : _originalCrosshairColor;
        }
    }
}