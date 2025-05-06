using System;
using CoreModule.Input;
using DG.Tweening;
using Enemy;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace TakanashiExtended
{
    public class ScalePlayer : MonoBehaviour
    {
        [SerializeField] private float minScale = 0.5f;
        [SerializeField] private float scaleFactor = 2f;
        [SerializeField] private float scaleDuration = 0.5f;
        [SerializeField] private AnimationCurve moveDuration;
        [SerializeField] private float bouncePower = 0.5f;
        [SerializeField] private float movePower = 0.5f;
        [SerializeField] private float assistRadius = 0.5f;
        [SerializeField] private bool normalDirection;
        [SerializeField] private AnimationCurve movePowerCurve;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private CinemachineFollow cinemachineFollow;
        [SerializeField] private float allowScaleTime = 5f;
        [SerializeField] private Transform attackArrow;

        private int moveStep;
        private float defaultScale = 1f;
        private float currentScale = 1f;
        private Tween currentTween;
        private Tween currentCameraTween;
        private bool isBigScaling;
        private float defaultCameraOffset;
        private InputEvent moveEvent;
        private Vector2 latestDirection;

        private void Start()
        {
            defaultScale = transform.localScale.x;
            currentScale = transform.localScale.x;
            defaultCameraOffset = cinemachineFollow.FollowOffset.z;

            InputEvent scaleShoot = InputActionProvider.CreateEvent(ActionGuid.Player.ScaleShoot);
            scaleShoot.Performed += OnScaleShoot;

            InputEvent unScaleShoot = InputActionProvider.CreateEvent(ActionGuid.Player.Jump);
            unScaleShoot.Performed += OnUnScaleShoot;

            InputEvent jumpEvent = InputActionProvider.CreateEvent(ActionGuid.Player.ResetBody);
            jumpEvent.Performed += OnResetScaleShoot;

            moveEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Move);
        }

        private void Update()
        {
            Vector2 inputDirection = moveEvent.ReadValue<Vector2>().normalized;

            if (inputDirection.sqrMagnitude > 0f)
            {
                bool isHit = Physics.SphereCast(transform.position, assistRadius, inputDirection, out RaycastHit hit, 100f);

                if (isHit && hit.transform.GetComponent<Status>() != null)
                {
                    inputDirection = (hit.transform.position - transform.position).normalized;
                }

                float rad = Mathf.Atan2(inputDirection.y, inputDirection.x);
                attackArrow.localRotation = Quaternion.Euler(0f, 0f, rad * Mathf.Rad2Deg - 90f);
            }
        }

        private void OnScaleShoot(InputAction.CallbackContext ctx)
        {
            currentScale = Mathf.Max(currentScale * scaleFactor, minScale);
            isBigScaling = true;
            DoScale();
            currentTween.OnComplete(() => { isBigScaling = false; });
        }

        private void OnUnScaleShoot(InputAction.CallbackContext ctx)
        {
            currentScale = Mathf.Max(currentScale * (1 / scaleFactor), minScale);

            if (currentScale > minScale && allowScaleTime > 0f)
            {
                Vector2 inputDirection = moveEvent.ReadValue<Vector2>().normalized;

                if (normalDirection)
                {
                    inputDirection = latestDirection;
                }

                playerMovement.SetLimitedForce(inputDirection * movePower * movePowerCurve.Evaluate(moveStep), moveDuration.Evaluate(currentScale));
                DoScale();
                moveStep += 1;
            }
        }

        public float EvaluateStep()
        {
            return movePowerCurve.Evaluate(moveStep);
        }

        private void OnResetScaleShoot(InputAction.CallbackContext ctx)
        {
            if (currentScale == defaultScale)
            {
                return;
            }

            float targetScale = currentScale;
            currentScale = defaultScale;

            if (targetScale > defaultScale)
            {
                // playerMovement.AddExternalForce(playerMovement.ExternalForce * 0.5f + playerMovement.ExternalForce.normalized * minBounce);
            }
            else
            {
                isBigScaling = true;
                currentTween.OnComplete(() => { isBigScaling = false; });
            }

            DoScale();
        }

        private void DoScale()
        {
            currentTween?.Complete();
            currentTween = transform.DOScale(currentScale, scaleDuration);

            currentCameraTween?.Complete();
            currentCameraTween = DOTween.To(
                () => cinemachineFollow.FollowOffset.z,
                x => cinemachineFollow.FollowOffset = new Vector3(
                    cinemachineFollow.FollowOffset.x,
                    cinemachineFollow.FollowOffset.y,
                    x),
                defaultCameraOffset - (currentScale - defaultScale) * 2.2f,
                scaleDuration);
        }

        private void OnCollisionStay(Collision other)
        {
            if (isBigScaling)
            {
                latestDirection = other.GetContact(0).normal;
                playerMovement.AddExternalForce(latestDirection * bouncePower);
                isBigScaling = false;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                moveStep = 0;
            }
        }

        private bool IsGround()
        {
            const float checkDistance = 1.05f;
            return Physics.BoxCast(transform.position, transform.localScale * 0.5f, -transform.up, Quaternion.identity, checkDistance);
        }
    }
}