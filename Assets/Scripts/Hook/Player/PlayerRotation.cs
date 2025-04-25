using System;
using CoreModule.Input;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerRotation : MonoBehaviour
    {
        [SerializeField] private Transform catchVisualizer;
        [SerializeField] private TextMeshProUGUI leftStickOnlyMode;

        public Vector2 LookInput => lookInput;
        private Vector2 lookInput;
        private Vector2 lastSideInput;
        private bool enableMouseInput;
        private bool enableLeftStickInput;

        private void Start()
        {
            lookInput = Vector2.right;
            lastSideInput = Vector2.right;

            InputEvent moveEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Move);
            moveEvent.Started += OnMove;
            moveEvent.Performed += OnMove;
            moveEvent.Canceled += OnMove;

            InputEvent lookEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Look);
            lookEvent.Started += OnLook;
            lookEvent.Performed += OnLook;
            lookEvent.Canceled += OnLook;
        }


        private void Update()
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                enableMouseInput = !enableMouseInput;
            }

            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                enableLeftStickInput = !enableLeftStickInput;

                if (enableLeftStickInput)
                {
                    leftStickOnlyMode.text = "Left Stick Only Mode: true";
                }
                else
                {
                    leftStickOnlyMode.text = "Left Stick Only Mode: false";
                }
            }

            if (enableMouseInput)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
                lookInput = (mousePosition - screenPoint).normalized;
            }

            UpdateRotation();
        }

        private void UpdateRotation()
        {
            // 視点のオブジェクト入力の方向に向ける
            float angle = Mathf.Atan2(lookInput.y, lookInput.x) * Mathf.Rad2Deg;
            catchVisualizer.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void OnMove(InputAction.CallbackContext ctx)
        {
            if (!enableLeftStickInput)
                return;

            Vector2 input = ctx.ReadValue<Vector2>();
            float x = input.x;
            bool isDeadZone = Mathf.Abs(x) > 0.3f;
            x = isDeadZone ? x : 0f;
            input.x = x;

            // ゼロ入力は無効
            if (input == Vector2.zero)
            {
                lookInput = lastSideInput;
            }
            else if (input.y > 0.5f && Vector2.Angle(input, Vector2.up) < 10f)
            {
                lookInput = Vector2.up;
            }
            else if (input.y < -0.5f && Vector2.Angle(input, Vector2.down) < 10f)
            {
                lookInput = Vector2.down;
            }
            else if (isDeadZone)
            {
                lookInput = input.x > 0f ? Vector2.right : Vector2.left;
                lastSideInput = lookInput;
            }
        }

        private void OnLook(InputAction.CallbackContext ctx)
        {
            if (enableLeftStickInput)
                return;

            Vector2 input = ctx.ReadValue<Vector2>();
            if (input != Vector2.zero)
            {
                lookInput = input.normalized;
            }
        }
    }
}