using System;
using System.Threading.Tasks;
using DG.Tweening;
using Enemy;
using UnityEngine;
using UnityEngine.VFX;

public class Plate : MonoBehaviour
{
    [SerializeField] private Status myStatus;
    [SerializeField] private VisualEffect explosionEffect;
    [SerializeField] private Collider collider;
    [SerializeField] private Transform rendererParent;

    private void Start()
    {
        myStatus.OnDamage += () =>
        {
            rendererParent.DOShakePosition(0.5f, 0.4f, 50);
        };

        myStatus.OnDeath += async () =>
        {
            explosionEffect.Play();
            collider.enabled = false;
            rendererParent.gameObject.SetActive(false);

            await Task.Delay(2000, destroyCancellationToken);

            Destroy(gameObject);
        };
    }
}