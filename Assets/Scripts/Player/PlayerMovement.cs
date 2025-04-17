using System;
using CoreModule.Input;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveAccel;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float damping;
        [SerializeField] private Vector2 externalDamping;
        [SerializeField] private float gravity;
        [SerializeField] private float jumpPower;

        [SerializeField] private Rigidbody rb;
        [SerializeField] private Vector2 externalForce = Vector2.zero;

        private float moveInput = 0f;

        private void Start()
        {
            InputEvent moveEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Move);
            moveEvent.Started += OnMove;
            moveEvent.Performed += OnMove;
            moveEvent.Canceled += OnMove;

            InputEvent jumpEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Jump);
            jumpEvent.Started += OnJump;
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            moveInput = ctx.ReadValue<Vector2>().x;
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (!IsGround())
                return;

            // 重力を初期化して力を加える
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f);
            rb.AddForce(new Vector3(0f, jumpPower), ForceMode.Impulse);
        }

        private bool IsGround()
        {
            const float checkDistance = 1.05f;
            return Physics.BoxCast(transform.position, transform.localScale * 0.5f, -transform.up, Quaternion.identity, checkDistance);
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