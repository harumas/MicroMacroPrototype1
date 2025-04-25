using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class EnemyVibrator : MonoBehaviour
    {
        [SerializeField] private float vibrationDuration = 0.5f;
        [FormerlySerializedAs("enemyStatus")] [SerializeField] private Status status;

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