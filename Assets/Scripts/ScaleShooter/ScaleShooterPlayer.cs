using System;
using System.Buffers;
using CoreModule.Input;
using ScaleShooter;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScaleShooterPlayer : MonoBehaviour
{
    [SerializeField] private float power;
    [SerializeField] private bool doFlip;
    [SerializeField] private float detectRadius;
    [SerializeField] private Vector3 shootDirection;
    [SerializeField] private GameObject scaleBulletPrefab;
    [SerializeField] private GameObject unscaleBulletPrefab;
    [SerializeField] private ShootPointVisualizer shootPointVisualizer;
    [SerializeField] private Transform pivot;

    private InputEvent moveEvent;
    private float defaultDirection;
    private float direction;
    private Vector3 pivotDirection;

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
        DetectPivot();

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

    private void DetectPivot()
    {
        Vector2 moveInput = moveEvent.ReadValue<Vector2>();
        (float min, float max) angleRange = (0f, 0f);

        if (moveInput.y < -0.5f)
        {
            angleRange = (-120f, 0f);
        }
        else
        {
            angleRange = (0f, 120f);
        }

        // 周囲にあるオブジェクトを取得
        var colliders = ArrayPool<Collider>.Shared.Rent(16);
        int count = Physics.OverlapSphereNonAlloc(transform.position, detectRadius, colliders);

        Vector3 pivot = Vector3.zero;
        float minDistance = float.MaxValue;
        bool found = false;

        foreach (Collider touchCollider in colliders.AsSpan(0, count))
        {
            if (!touchCollider.TryGetComponent(out ScalePivot scalePivot))
                continue;

            Vector2 diff = scalePivot.transform.position - transform.position;

            float angle = Vector2.SignedAngle(Vector2.right * direction, diff) * direction;
            float distance = diff.magnitude;

            if (angleRange.min <= angle && angle < angleRange.max && distance < minDistance)
            {
                minDistance = distance;
                pivot = scalePivot.transform.position;
                found = true;
            }
        }

        if (found)
        {
            shootPointVisualizer.Visualize(pivot);
            pivotDirection = (pivot - this.pivot.position).normalized;
        }
        else
        {
            shootPointVisualizer.Hide();
            pivotDirection = Vector3.zero;
        }

        ArrayPool<Collider>.Shared.Return(colliders);
    }

    private void OnScaleShoot(InputAction.CallbackContext obj)
    {
        if (pivotDirection == Vector3.zero)
            return;

        Rigidbody bullet = Instantiate(scaleBulletPrefab, pivot.position, Quaternion.identity).GetComponent<Rigidbody>();
        bullet.AddForce(pivotDirection * power);
    }

    private void OnUnscaleShoot(InputAction.CallbackContext obj)
    {
        if (pivotDirection == Vector3.zero)
            return;

        Rigidbody bullet = Instantiate(unscaleBulletPrefab, pivot.position, Quaternion.identity).GetComponent<Rigidbody>();
        bullet.AddForce(pivotDirection * power);
    }
}