using CoreModule.Input;
using Cross;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float shootPower;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float shootInterval;
    [SerializeField] private Transform shootPointVisualizer;
    [SerializeField] private Transform shootPivot;
    [SerializeField] private GameObject scaleBulletPrefab;
    [SerializeField] private GameObject unscaleBulletPrefab;

    public Vector2 LookInput => lookInput;
    private Vector2 lookInput;
    private Vector2 lastSideInput;
    private Vector3 defaultGunScale;
    private bool enableMouseInput;
    private bool isCharging;
    private float shootTime;
    private float lastShootTime;
    private Tween scaleTween;

    private void Start()
    {
        lookInput = Vector2.right;
        lastSideInput = Vector2.right;
        defaultGunScale = shootPivot.localScale;

        InputEvent moveEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Move);
        moveEvent.Started += OnMove;
        moveEvent.Performed += OnMove;
        moveEvent.Canceled += OnMove;

        InputEvent catchEvent = InputActionProvider.CreateEvent(ActionGuid.Player.ScaleShoot);
        catchEvent.Performed += OnStartShoot;
        catchEvent.Canceled += OnScaleShoot;

        InputEvent shootEvent = InputActionProvider.CreateEvent(ActionGuid.Player.UnScaleShoot);
        shootEvent.Performed += OnStartShoot;
        shootEvent.Canceled += OnUnscaleShoot;
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

        bool isCharged = Time.time - shootTime > chargeDuration;
        if (isCharging && isCharged)
        {
            scaleTween = shootPivot.DOScale(defaultGunScale * 2f, 0.5f).SetEase(Ease.OutBack, 3f);
        }
        else
        {
            scaleTween?.Kill();
            shootPivot.localScale = defaultGunScale;
        }

        UpdateRotation();
    }

    private void UpdateRotation()
    {
        // 視点のオブジェクト入力の方向に向ける
        float angle = Mathf.Atan2(lookInput.y, lookInput.x) * Mathf.Rad2Deg;
        shootPointVisualizer.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
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

    private void OnStartShoot(InputAction.CallbackContext obj)
    {
        if (lastShootTime + shootInterval > Time.time)
            return;
        
        shootTime = Time.time;
        isCharging = true;
    }

    private void OnScaleShoot(InputAction.CallbackContext obj)
    {
        if (!isCharging)
            return;

        bool isCharged = Time.time - shootTime > chargeDuration;

        GameObject bulletObject = Instantiate(scaleBulletPrefab, shootPivot.position, Quaternion.identity);

        ScaleBullet bullet = bulletObject.GetComponent<ScaleBullet>();
        bullet.SetCharged(isCharged);

        Rigidbody bulletRigidbody = bulletObject.GetComponent<Rigidbody>();
        bulletRigidbody.AddForce(lookInput * shootPower);
        isCharging = false;
        lastShootTime = Time.time;
    }

    private void OnUnscaleShoot(InputAction.CallbackContext obj)
    {
        if (!isCharging)
            return;

        bool isCharged = Time.time - shootTime > chargeDuration;

        GameObject bulletObject = Instantiate(unscaleBulletPrefab, shootPivot.position, Quaternion.identity);

        ScaleBullet bullet = bulletObject.GetComponent<ScaleBullet>();
        bullet.SetCharged(isCharged);

        Rigidbody bulletRigidbody = bulletObject.GetComponent<Rigidbody>();
        bulletRigidbody.AddForce(lookInput * shootPower);
        isCharging = false;
        lastShootTime = Time.time;
    }
}