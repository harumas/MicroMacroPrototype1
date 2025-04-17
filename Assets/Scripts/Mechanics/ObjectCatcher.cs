using System;
using System.Buffers;
using Constants;
using CoreModule.Input;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class ObjectCatcher : MonoBehaviour
    {
        [SerializeField] private float catchRadius;
        [SerializeField] private float catchPivotRadius;
        [SerializeField] private float accmulateTime = 1.5f;
        [SerializeField] private PlayerRotation playerRotation;

        private CatchableObject currentObject;
        private float catchTime;
        private bool isShooting;

        public float CalculateScale()
        {
            return Mathf.Min(1f, Mathf.Pow(Time.time - catchTime, 3) / accmulateTime);
        }

        private void Start()
        {
            InputEvent catchEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Catch);
            catchEvent.Started += OnCatch;
            catchEvent.Canceled += OnCatch;

            InputEvent shootEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Shoot);
            shootEvent.Started += OnShoot;
        }

        private void Update()
        {
            // キャッチしたオブジェクト座標をプレイヤーに合わせる
            if (currentObject != null && !isShooting)
            {
                Vector3 position = (Vector3)playerRotation.LookInput * catchPivotRadius + transform.position;
                currentObject.Sync(position);
            }
        }

        private void OnCatch(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                Catch();
            }
            else if (ctx.canceled)
            {
                Release();
            }
        }

        private void OnShoot(InputAction.CallbackContext ctx)
        {
            if (currentObject == null || isShooting)
                return;

            Vector3 direction = playerRotation.LookInput;
            currentObject.Shoot(direction);
            isShooting = true;
        }

        private void Catch()
        {
            if (TryCatch(playerRotation.LookInput, out CatchableObject catchableObject))
            {
                currentObject = catchableObject;
                currentObject.Catch();

                catchTime = Time.time;
            }
        }

        private void Release()
        {
            if (currentObject == null)
                return;

            float scale = CalculateScale();

            currentObject.Release(scale);
            currentObject = null;
            isShooting = false;
        }

        private bool TryCatch(Vector2 lookInput, out CatchableObject catchableObject)
        {
            // デフォルトはnull
            catchableObject = null;

            // プレイヤーの周囲にあるオブジェクトを取得
            var colliders = ArrayPool<Collider>.Shared.Rent(16);
            int count = Physics.OverlapSphereNonAlloc(transform.position, catchRadius, colliders, Layer.Mask.Catchable);

            GameObject nearestObject = null;
            float nearestDot = float.MaxValue;

            // 視線の周囲にあるオブジェクトを探索
            foreach (Collider col in colliders.AsSpan(0, count))
            {
                Vector2 targetVec = col.transform.position - transform.position;

                float dot = Vector3.Dot(lookInput, targetVec);
                if (dot > 0 && dot < nearestDot)
                {
                    nearestDot = dot;
                    nearestObject = col.gameObject;
                }
            }

            ArrayPool<Collider>.Shared.Return(colliders);
            return nearestObject != null && nearestObject.TryGetComponent(out catchableObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, catchRadius);
        }
    }
}