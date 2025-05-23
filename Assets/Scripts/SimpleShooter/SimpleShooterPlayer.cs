using System;
using System.Buffers;
using CoreModule.Input;
using ScaleShooter;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleShooterPlayer : MonoBehaviour
{
    [SerializeField] private float power;
    [SerializeField] private bool doFlip;
    [SerializeField] private Vector3 shootDirection;
    [SerializeField] private GameObject scaleBulletPrefab;
    [SerializeField] private GameObject unscaleBulletPrefab;
    [SerializeField] private Transform pivot;

    private InputEvent moveEvent;
    private float defaultDirection;
    private float direction;

    private void Start()
    {
        InputEvent catchEvent = InputActionProvider.CreateEvent(ActionGuid.Player.ScaleShoot);
        catchEvent.Performed += OnScaleShoot;

        InputEvent shootEvent = InputActionProvider.CreateEvent(ActionGuid.Player.UnScaleShoot);
        shootEvent.Performed += OnUnscaleShoot;

        InputEvent switchControlEvent = InputActionProvider.CreateEvent(ActionGuid.Player.SwitchControl);
        switchControlEvent.Performed += OnSwitchControl;

        moveEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Move);
        defaultDirection = pivot.transform.localPosition.x;
        direction = 1f;
    }

    private void OnSwitchControl(InputAction.CallbackContext ctx)
    {
        doFlip = !doFlip;
    }

    private void Update()
    {
        if (!doFlip)
            return;

        Vector2 moveInput = moveEvent.ReadValue<Vector2>();

        if (moveInput != Vector2.zero && Mathf.Abs(moveInput.x) >= 0.3f)
        {
            Vector3 position = pivot.transform.localPosition;
            direction = Mathf.Sign(moveInput.x);
            position.x = defaultDirection * direction;
            pivot.transform.localPosition = position;
        }
    }

    private void OnScaleShoot(InputAction.CallbackContext obj)
    {
        Rigidbody bullet = Instantiate(scaleBulletPrefab, pivot.position, Quaternion.identity).GetComponent<Rigidbody>();
        bullet.AddForce(shootDirection * direction * power);
    }

    private void OnUnscaleShoot(InputAction.CallbackContext obj)
    {
        Rigidbody bullet = Instantiate(unscaleBulletPrefab, pivot.position, Quaternion.identity).GetComponent<Rigidbody>();
        bullet.AddForce(shootDirection * direction * power);
    }
}