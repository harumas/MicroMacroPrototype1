using System;
using CoreModule.Input;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Takanashi
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody rig;
        [SerializeField] private ScalePlayer scalePlayer;
        [SerializeField] private float damping;
        [SerializeField] private float moveAccel;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float gravity;

        private Vector2 limitedForce;
        private Vector2 moveInput;
        private Vector3 externalForce;
        private bool isSonicSpeed;
        private float scalingTime;
        private Tween forceTween;

        public bool IsMoving;

        private void Start()
        {
            InputEvent unScaleShoot = InputActionProvider.CreateEvent(ActionGuid.Player.Jump);
            unScaleShoot.Canceled += OnUnScaleShoot;

            InputEvent moveEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Move);
            moveEvent.Started += OnMove;
            moveEvent.Performed += OnMove;
            moveEvent.Canceled += OnMove;
        }

        private void OnUnScaleShoot(InputAction.CallbackContext _)
        {
            scalingTime = 0f;
        }

        public void SetLimitedForce(Vector2 force, float time)
        {
            limitedForce = force;
            scalingTime = time;
            IsMoving = true;
        }

        private void FixedUpdate()
        {
            Vector3 velocity = rig.linearVelocity;

            if (limitedForce.sqrMagnitude > 0f && scalingTime <= 0f)
            {
                limitedForce *= damping;

                if (limitedForce.sqrMagnitude <= 0.1f)
                {
                    limitedForce = Vector2.zero;
                    IsMoving = false;
                }
            }

            if (limitedForce.sqrMagnitude > 0f && scalePlayer.EvaluateStep() > 0f)
            {
                velocity = limitedForce;
            }
            else
            {
                PerformMove(ref velocity);
            }

            velocity += Vector3.up * gravity;
            velocity += externalForce;

            velocity.x *= damping;
            externalForce *= damping;
            scalingTime -= Time.fixedDeltaTime;
            rig.linearVelocity = velocity;
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            moveInput = ctx.ReadValue<Vector2>();
        }

        private void PerformMove(ref Vector3 velocity)
        {
            velocity.x = Mathf.Min(maxSpeed, velocity.x + moveInput.x * moveAccel);
        }

        public void AddExternalForce(Vector3 bouncePower)
        {
            externalForce += bouncePower;
        }
    }
}