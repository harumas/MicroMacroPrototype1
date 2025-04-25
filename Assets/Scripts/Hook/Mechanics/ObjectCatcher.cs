using System;
using System.Buffers;
using Constants;
using CoreModule.Input;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class ObjectCatcher : MonoBehaviour
    {
        [SerializeField] private float catchRadius;
        [SerializeField, Range(0f, 360f)] private float catchAngleRange;
        [SerializeField] private float catchPivotRadius;
        [SerializeField] private float accmulateTime = 1.5f;
        [SerializeField] private int increaseStep = 3;
        [SerializeField] private bool isVerticalClamp;
        [SerializeField] private bool isHoldControl = true;

        [SerializeField] private PlayerRotation playerRotation;
        [SerializeField] private CircleGauge circleGauge;
        [SerializeField] private CircularSectorMeshRenderer visualizer;
        [SerializeField] private TextMeshProUGUI controlModeText;

        private InputEvent catchEvent;
        private CatchableObject currentObject;
        private Tween shakeTween;
        private Vector3 shakeVector;
        private float catchTime;
        private float shootTime;
        private bool isShooting;

        public float CalculateScale()
        {
            float currentTime = isShooting ? shootTime : Time.time;
            float timeRate = Mathf.Min(accmulateTime, currentTime - catchTime) / accmulateTime;
            float stepRate = 1f / (increaseStep - 1);

            return Mathf.Floor(timeRate / stepRate) * stepRate;
        }

        private void Start()
        {
            catchEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Catch);
            catchEvent.Started += OnCatchStarted;
            catchEvent.Canceled += OnCatchCanceled;

            InputEvent shootEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Shoot);
            shootEvent.Started += OnShoot;

            InputEvent switchEvent = InputActionProvider.CreateEvent(ActionGuid.Player.SwitchControl);
            switchEvent.Started += SwitchControl;

            visualizer.RegenerateMesh(catchAngleRange, catchRadius);
        }

        private void Update()
        {
            // キャッチしたオブジェクト座標をプレイヤーに合わせる
            if (currentObject != null && !isShooting)
            {
                Vector3 position = (Vector3)playerRotation.LookInput * catchPivotRadius + transform.position + shakeVector;
                currentObject.Sync(position);
            }

            float stepRate = 1f / increaseStep;
            circleGauge.SetGauge(currentObject == null ? 0f : (1f - stepRate) * CalculateScale(), stepRate);
            circleGauge.ShowBackground(currentObject != null);
        }

        private void SwitchControl(InputAction.CallbackContext ctx)
        {
            catchEvent.Started -= OnCatchStarted;
            catchEvent.Started -= OnCatchCanceled;
            catchEvent.Canceled -= OnCatchCanceled;
            isHoldControl = !isHoldControl;

            if (isHoldControl)
            {
                catchEvent.Started += OnCatchStarted;
                catchEvent.Canceled += OnCatchCanceled;
                controlModeText.text = "操作モード: 長押し";
            }
            else
            {
                catchEvent.Started += OnCatchStarted;
                catchEvent.Started += OnCatchCanceled;
                controlModeText.text = "操作モード: 切り替え";
            }
        }

        private void OnCatchStarted(InputAction.CallbackContext ctx)
        {
            if (currentObject == null)
            {
                Catch();
            }
        }

        private void OnCatchCanceled(InputAction.CallbackContext ctx)
        {
            if (currentObject != null && Math.Abs(catchTime - Time.time) > Time.deltaTime)
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
            shootTime = Time.time;
        }

        private void Catch()
        {
            if (TryCatch(playerRotation.LookInput, out CatchableObject catchableObject))
            {
                currentObject = catchableObject;
                currentObject.Catch();

                catchTime = Time.time;
                shakeTween?.Kill();
                shakeTween = DOTween.Shake(() => shakeVector, v => shakeVector = v, 1f, 0.05f, 40, 90f, true, false).SetLoops(-1);
            }
        }

        private void Release()
        {
            if (currentObject == null)
                return;

            float scale = CalculateScale();


            shakeTween?.Rewind();
            shakeTween?.Kill();

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
            float nearestAngle = float.MaxValue;

            // 視線の周囲にあるオブジェクトを探索
            foreach (Collider col in colliders.AsSpan(0, count))
            {
                Vector2 targetVec = col.transform.position - transform.position;

                float angle = Vector3.Angle(lookInput, targetVec);
                if (angle <= catchAngleRange && angle < nearestAngle)
                {
                    nearestAngle = angle;
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