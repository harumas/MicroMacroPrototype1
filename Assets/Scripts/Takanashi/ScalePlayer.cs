using System;
using CoreModule.Input;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Takanashi
{
    public class ScalePlayer : MonoBehaviour
    {
        [SerializeField] private float minScale = 0.5f;
        [SerializeField] private float scaleFactor = 2f;
        [SerializeField] private float scaleDuration = 0.5f;
        [SerializeField] private float bouncePower = 0.5f;
        [SerializeField] private float minBounce = 4f;
        [SerializeField] private float unScaleFactor = 0.5f;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private CinemachineFollow cinemachineFollow;

        private float defaultScale = 1f;
        private float currentScale = 1f;
        private Tween currentTween;
        private Tween currentCameraTween;
        private bool isBigScaling;
        private float defaultCameraOffset;

        private void Start()
        {
            defaultScale = transform.localScale.x;
            currentScale = transform.localScale.x;
            defaultCameraOffset = cinemachineFollow.FollowOffset.z;

            InputEvent scaleShoot = InputActionProvider.CreateEvent(ActionGuid.Player.ScaleShoot);
            scaleShoot.Performed += OnScaleShoot;

            InputEvent unScaleShoot = InputActionProvider.CreateEvent(ActionGuid.Player.UnScaleShoot);
            unScaleShoot.Performed += OnUnScaleShoot;

            InputEvent jumpEvent = InputActionProvider.CreateEvent(ActionGuid.Player.ResetBody);
            jumpEvent.Performed += OnResetScaleShoot;
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

            if (currentScale > minScale)
            {
                playerMovement.AddExternalForce(playerMovement.ExternalForce * 0.5f + playerMovement.ExternalForce.normalized * minBounce);
                DoScale();
            }
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
                playerMovement.AddExternalForce(playerMovement.ExternalForce * 0.5f + playerMovement.ExternalForce.normalized * minBounce);
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
                defaultCameraOffset - (currentScale - defaultScale) * 0.9f,
                scaleDuration);
        }

        private void OnCollisionStay(Collision other)
        {
            if (isBigScaling)
            {
                playerMovement.AddExternalForce(other.GetContact(0).normal * bouncePower);
                isBigScaling = false;
            }
        }

        private bool IsGround()
        {
            const float checkDistance = 1.05f;
            return Physics.BoxCast(transform.position, transform.localScale * 0.5f, -transform.up, Quaternion.identity, checkDistance);
        }
    }
}