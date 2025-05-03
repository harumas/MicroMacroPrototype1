using System;
using CoreModule.Input;
using UGizmo;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Takanashi3D 
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveAccel;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float damping;
        [SerializeField] private Vector2 externalDamping;
        [SerializeField] private float gravity;
        [SerializeField] private float jumpPower;
        [SerializeField] private AnimationCurve additionalJumpPower;
        [SerializeField] private Transform bodyTransform;

        [SerializeField] private Rigidbody rb;
        [SerializeField] private Vector3 externalForce;

        private Vector2 moveInput;
        private float jumpStartTime = 0f;
        [SerializeField] private bool isJumping = false;

        public float MaxSpeed
        {
            get => maxSpeed;
            set => maxSpeed = value;
        }

        private void Start()
        {
            InputEvent moveEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Move);
            moveEvent.Started += OnMove;
            moveEvent.Performed += OnMove;
            moveEvent.Canceled += OnMove;

            InputEvent jumpEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Jump);
            jumpEvent.Started += OnJump;
            jumpEvent.Canceled += OnJumpEnd;
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            moveInput = ctx.ReadValue<Vector2>();
        }

        private void Update()
        {
            if (moveInput != Vector2.zero)
            {
                Vector3 forward = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
                Quaternion target = Quaternion.LookRotation(forward, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * 16f);
            }
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (!IsGround())
                return;

            // 重力を初期化して力を加える
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(new Vector3(0f, jumpPower, 0f), ForceMode.VelocityChange);
            isJumping = true;
            jumpStartTime = Time.time;
        }

        private void OnJumpEnd(InputAction.CallbackContext obj)
        {
            isJumping = false;
        }

        private bool IsGround()
        {
            const float checkDistance = 0.7f;

            // 地面に触れた状態でBoxCastすると検出してくれないので、少し上にずらす
            Vector3 groundOffset = new Vector3(0f, 0.1f, 0f);

            return Physics.BoxCast(transform.position + groundOffset, transform.localScale * 0.5f, -transform.up, transform.rotation, checkDistance);
        }

        private void FixedUpdate()
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y += gravity; // 重力を加算

            PerformMove(ref velocity);
            PerformDamping(ref velocity);
            PerformExternalForce();

            if (isJumping)
            {
                // ジャンプ中の追加の力を加える
                float jumpTime = Time.time - jumpStartTime;
                float additionalPower = additionalJumpPower.Evaluate(jumpTime);
                velocity += new Vector3(0f, additionalPower, 0f);
            }

            rb.linearVelocity = velocity + externalForce;
        }

        private void PerformMove(ref Vector3 velocity)
        {
            // x方向に力を与える
            Vector3 v = new Vector3(moveInput.x, 0f, moveInput.y) * moveAccel;

            // Rigidbodyの速度を設定する
            velocity += v;
            Vector2 clampedVelocity = Vector2.ClampMagnitude(new Vector2(velocity.x, velocity.z), maxSpeed);
            velocity = new Vector3(clampedVelocity.x, velocity.y, clampedVelocity.y);
            
        }

        private void PerformDamping(ref Vector3 velocity)
        {
            // 速度を減少させる
            velocity.x *= damping;
            velocity.z *= damping;
        }

        private void PerformExternalForce()
        {
            externalForce *= externalDamping;

            if (externalForce.sqrMagnitude < 0.01f)
            {
                externalForce = Vector3.zero;
            }
        }

        public void AddExternalForce(Vector3 force)
        {
            externalForce += force;
        }

        private void OnDestroy()
        {
            InputActionProvider.ClearEvents();
        }
    }
}