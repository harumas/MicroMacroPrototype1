using System;
using DG.Tweening;
using Enemy;
using UnityEngine;

namespace Cross
{
    public class Barrier : MonoBehaviour
    {
        [SerializeField] private Status status;

        private void Start()
        {
            status.OnDeath += () => { transform.DOScale(0f, 0.3f).SetEase(Ease.OutBack, 3f).OnComplete(() => Destroy(gameObject)); };
        }
    }
}