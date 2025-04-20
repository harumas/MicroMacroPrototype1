using System;
using CoreModule.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerRotation : MonoBehaviour
    {
        [SerializeField] private Transform catchVisualizer;

        public Vector2 LookInput => lookInput;
        private Vector2 lookInput;
        private bool enableMouseInput;

        private void Start()
        {
            lookInput = Vector2.right;

            InputEvent lookEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Look);
            lookEvent.Performed += OnLook;
        }

        private void Update()
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                enableMouseInput = !enableMouseInput;
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

        private void OnLook(InputAction.CallbackContext ctx)
        {
            Vector2 input = ctx.ReadValue<Vector2>();

            // ゼロ入力は無効
            if (input == Vector2.zero)
                return;

            lookInput = input.normalized;
        }
    }
}