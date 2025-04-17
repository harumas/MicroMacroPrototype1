using DG.Tweening;
using UnityEngine;

namespace Enemy
{
    public class EnemyVibrator : MonoBehaviour
    {
        [SerializeField] private float vibrationDuration = 0.5f;
        [SerializeField] private EnemyStatus enemyStatus;

        private void Start()
        {
            enemyStatus.OnDamage += OnDamage;
            enemyStatus.OnDeath += OnDamage;
        }

        private void OnDamage()
        {
            transform.DOShakePosition(vibrationDuration);
        }
    }
}
