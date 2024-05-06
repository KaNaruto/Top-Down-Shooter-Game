using Combat.Skills;
using UnityEngine;

namespace LivingEntities.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private Vector3 _direction;
        public float moveSpeed = 5;
        private bool _isDashing;


        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();

            Dash.OnDashStart += StopMovementDuringDash;
            Dash.OnDashEnd += ResumeMovementAfterDash;
        }
    
        public void Move(Vector3 direction)
        {
            _direction = direction;
        }

        public void RotateTo(Vector3 rotatePoint)
        {
            if (_isDashing)
                return;
            Vector3 heightCorrectPoint = new Vector3(rotatePoint.x, transform.position.y, rotatePoint.z);
            transform.LookAt(heightCorrectPoint);
        }

        private void FixedUpdate()
        {
            if (_isDashing)
                return;

            Vector3 velocity = _direction * moveSpeed;
            _rigidbody.MovePosition(_rigidbody.position + velocity * Time.fixedDeltaTime);
        }
        private void StopMovementDuringDash()
        {
            _isDashing = true;
        }

        private void ResumeMovementAfterDash()
        {
            _isDashing = false;
        }
    }
}