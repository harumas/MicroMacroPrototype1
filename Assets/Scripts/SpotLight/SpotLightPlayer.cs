using System;
using System.Buffers;
using System.Collections.Generic;
using Constants;
using CoreModule.Input;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpotLightPlayer : MonoBehaviour
{
    [SerializeField] private float scaleSize;
    [SerializeField] private float scaleDuration;
    [SerializeField] private CircularSectorMeshRenderer circularMeshShort;
    [SerializeField] private CircularSectorMeshRenderer circularMeshLong;
    [SerializeField] private Transform pivot;

    private InputEvent moveEvent;
    private Vector2 lookInput;
    private bool isLong;
    private float detectRadius;
    private float detectAngleRange;

    private void Start()
    {
        lookInput = Vector2.right;

        circularMeshShort.gameObject.SetActive(!isLong);
        circularMeshLong.gameObject.SetActive(isLong);
        detectRadius = isLong ? circularMeshLong.Radius : circularMeshShort.Radius;
        detectAngleRange = isLong ? circularMeshLong.Degree : circularMeshShort.Degree;

        InputEvent spotEvent = InputActionProvider.CreateEvent(ActionGuid.Player.SpotLight);
        spotEvent.Started += _ =>
        {
            isLong = !isLong;
            circularMeshShort.gameObject.SetActive(!isLong);
            circularMeshLong.gameObject.SetActive(isLong);

            detectRadius = isLong ? circularMeshLong.Radius : circularMeshShort.Radius;
            detectAngleRange = isLong ? circularMeshLong.Degree : circularMeshShort.Degree;
        };

        InputEvent catchEvent = InputActionProvider.CreateEvent(ActionGuid.Player.ScaleShoot);
        catchEvent.Performed += OnScaleShoot;

        InputEvent shootEvent = InputActionProvider.CreateEvent(ActionGuid.Player.UnScaleShoot);
        shootEvent.Performed += OnUnscaleShoot;

        moveEvent = InputActionProvider.CreateEvent(ActionGuid.Player.Move);
    }

    private void Update()
    {
        Vector2 moveInput = moveEvent.ReadValue<Vector2>();

        if (moveInput != Vector2.zero && Mathf.Abs(moveInput.x) >= 0.3f)
        {
            lookInput = moveInput.x > 0f ? Vector2.right : Vector2.left;

            float angle = Mathf.Atan2(lookInput.y, lookInput.x) * Mathf.Rad2Deg;
            pivot.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private void OnScaleShoot(InputAction.CallbackContext obj)
    {
        if (TryGetScaleObjects(lookInput, detectRadius, detectAngleRange, out List<ScaleObject> scaleObjects))
        {
            foreach (var scaleObject in scaleObjects)
            {
                scaleObject.Scale(scaleSize, scaleDuration);
            }
        }
    }

    private void OnUnscaleShoot(InputAction.CallbackContext obj)
    {
        if (TryGetScaleObjects(lookInput, detectRadius, detectAngleRange, out List<ScaleObject> scaleObjects))
        {
            foreach (var scaleObject in scaleObjects)
            {
                scaleObject.Scale(-scaleSize, scaleDuration);
            }
        }
    }

    private bool TryGetScaleObjects(Vector2 lookInput, float radius, float angleRange, out List<ScaleObject> result)
    {
        // プレイヤーの周囲にあるオブジェクトを取得
        var colliders = ArrayPool<Collider>.Shared.Rent(16);
        int count = Physics.OverlapSphereNonAlloc(transform.position, radius, colliders);

        List<ScaleObject> scaleObjects = new List<ScaleObject>();

        // 視線の周囲にあるオブジェクトを探索
        foreach (Collider col in colliders.AsSpan(0, count))
        {
            Vector2 targetVec = col.transform.position - transform.position;

            float angle = Vector3.Angle(lookInput, targetVec);
            if (angle <= angleRange && col.TryGetComponent(out ScaleObject scaleObject))
            {
                scaleObjects.Add(scaleObject);
            }
        }

        result = scaleObjects;

        ArrayPool<Collider>.Shared.Return(colliders);
        return scaleObjects.Count > 0;
    }
}