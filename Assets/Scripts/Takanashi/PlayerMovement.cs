using CoreModule.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Takanashi
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveAccel;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float damping;
        [SerializeField] private Vector2 externalDamping;
        [SerializeField] private float gravity;

        [SerializeField] private Rigidbody rb;
        [SerializeField] private Vector2 externalForce = Vector2.zero;

        private float moveInput = 0f;
        public Vector2 ExternalForce => externalForce;

        private void Start()
        {
            InputEvent moveEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Move);
            moveEvent.Started += OnMove;
            moveEvent.Performed += OnMove;
            moveEvent.Canceled += OnMove;
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            float x = ctx.ReadValue<Vector2>().x;
            x = Mathf.Abs(x) > 0.3f ? x : 0f;
            moveInput = x;
        }


        private void FixedUpdate()
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.y += gravity; // 重力を加算

            PerformMove(ref velocity);
            PerformDamping(ref velocity);
            PerformExternalForce();

            rb.linearVelocity = velocity + externalForce;
        }

        private void PerformMove(ref Vector2 velocity)
        {
            // x方向に力を与える
            float vx = moveInput * moveAccel;

            // Rigidbodyの速度を設定する
            velocity.x += vx;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }

        private void PerformDamping(ref Vector2 velocity)
        {
            // 速度を減少させる
            velocity.x *= damping;
        }

        private void PerformExternalForce()
        {
            externalForce *= externalDamping;

            if (externalForce.sqrMagnitude < 0.01f)
            {
                externalForce = Vector3.zero;
            }
        }

        public void AddExternalForce(Vector2 force)
        {
            externalForce += force;
        }

        private void OnDestroy()
        {
            InputActionProvider.ClearEvents();
        }
    }
}