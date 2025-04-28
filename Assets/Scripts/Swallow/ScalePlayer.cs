using System;
using CoreModule.Input;
using DG.Tweening;
using Enemy;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Swallow
{
    public enum ScaleState
    {
        Normal,
        Big
    }

    public class ScalePlayer : MonoBehaviour
    {
        [SerializeField] private float scaleDuration = 0.5f;
        [SerializeField] private float maxScale = 3f;
        [SerializeField] private float speedOnMax = 5f;
        [SerializeField] private Rigidbody rig;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private CinemachineFollow cinemachineFollow;
        [SerializeField] private LockAxisCamera lockAxisCamera;

        private Vector3 defaultScale;
        private Tween currentScaleTween;
        private Tween currentCameraTween;
        private float defaultSpeed;
        private Vector3 defaultOffsetDirection;
        private float defaultCameraDistance;

        public ScaleState ScaleState = ScaleState.Normal;

        private void Start()
        {
            defaultScale = transform.localScale;
            defaultSpeed = playerMovement.MaxSpeed;
            defaultCameraDistance = (cinemachineFollow.transform.position - transform.position).magnitude;
            defaultOffsetDirection = (cinemachineFollow.transform.position - transform.position).normalized;

            InputEvent unscaleEvent = InputActionProvider.CreateEvent(ActionGuid.Player.ToBig);
            unscaleEvent.Started += OnStartScale;
            unscaleEvent.Canceled += OnEndScale;
        }

        private void OnStartScale(InputAction.CallbackContext _)
        {
            DoUpScale();
        }

        private void OnEndScale(InputAction.CallbackContext _)
        {
            DoInitialScale();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (ScaleState != ScaleState.Big ||
                other.GetContact(0).normal != Vector3.up ||
                !other.gameObject.TryGetComponent(out Status status))
                return;


            status.Damage(20);
        }

        private void DoUpScale()
        {
            ScaleState = ScaleState.Big;
            currentScaleTween?.Kill();
            currentCameraTween?.Kill();
            currentScaleTween = transform.DOScale(maxScale, scaleDuration).SetEase(Ease.OutBack, 3f);
            currentCameraTween = DoFollowCamera(+2f);
            playerMovement.MaxSpeed = speedOnMax;
            rig.mass = 100f;
        }

        private void DoInitialScale()
        {
            ScaleState = ScaleState.Normal;
            currentScaleTween?.Kill();
            currentCameraTween?.Kill();
            currentScaleTween = transform.DOScale(defaultScale, scaleDuration).SetEase(Ease.OutBack, 2f);
            currentCameraTween = DoFollowCamera(0f);
            playerMovement.MaxSpeed = defaultSpeed;
            rig.mass = 10f;
        }

        private Tween DoFollowCamera(float offset)
        {
            return DOTween.To(
                () => new Vector3(cinemachineFollow.FollowOffset.x, lockAxisCamera.lockPosition.y, cinemachineFollow.FollowOffset.z),
                v =>
                {
                    cinemachineFollow.FollowOffset = new Vector3(v.x, v.y, v.z);
                    lockAxisCamera.lockPosition = new Vector3(0f, v.y, 0f);
                }, defaultOffsetDirection * (defaultCameraDistance + offset)
                , scaleDuration).SetEase(Ease.OutBack, 2f);
        }
    }
}