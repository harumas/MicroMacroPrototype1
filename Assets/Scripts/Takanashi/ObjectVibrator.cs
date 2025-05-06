using System;
using DG.Tweening;
using Enemy;
using UnityEngine;

namespace Takanashi
{
    public class ObjectVibrator : MonoBehaviour
    {
        [SerializeField] private float bouncePower = 5f;
        [SerializeField] private float vibrationDuration = 1f;
        [SerializeField] private Status status;

        private void Start()
        {
            status.OnDamage += OnDamage;
            status.OnDeath += OnDamage;
        }

        private void OnDamage()
        {
            transform.DOShakePosition(vibrationDuration, 1f, 20);
        }
    }
}