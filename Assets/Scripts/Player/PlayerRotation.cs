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

        private void Start()
        {
            lookInput = (catchVisualizer.position - transform.position).normalized;

            InputEvent lookEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Look);
            lookEvent.Performed += OnLook;
        }

        private void OnLook(InputAction.CallbackContext ctx)
        {
            Vector2 input = ctx.ReadValue<Vector2>();

            // ゼロ入力は無効
            if (input == Vector2.zero)
                return;

            lookInput = input.normalized;

            // 視点のオブジェクト入力の方向に向ける
            float angle = Mathf.Atan2(lookInput.y, lookInput.x) * Mathf.Rad2Deg;
            catchVisualizer.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}