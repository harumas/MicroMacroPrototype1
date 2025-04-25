using System;
using System.Buffers;
using Constants;
using DG.Tweening;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class FlyingEnemy : MonoBehaviour
    {
        [SerializeField] private float damage;
        [SerializeField] private float shootPower;
        [SerializeField] private float shootInterval;
        [SerializeField] private float detectRadius;
        [SerializeField] private float angleRange;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private CatchableObject catchableObject;
        [SerializeField] private Status status;
        [SerializeField] private Renderer bodyRenderer;

        private void Start()
        {
            InvokeRepeating(nameof(Attack), shootInterval, shootInterval);

            status.OnDeath += () => { catchableObject.Release(0.5f); };

            catchableObject.OnRelease += OnRelease;
        }

        private void OnRelease(float releasedScale)
        {
            if (catchableObject.CurrentState == CatchableObject.State.PreRelease)
            {
                ExplodeDamage(releasedScale);
            }

            Material material = bodyRenderer.material;
            material.DOFade(0f, 0.5f).OnComplete(() => { Destroy(gameObject); });
        }

        private void ExplodeDamage(float releasedScale)
        {
            var colliders = ArrayPool<Collider>.Shared.Rent(16);
            int count = Physics.OverlapSphereNonAlloc(transform.position, releasedScale * 0.5f + 0.5f, colliders);

            foreach (Collider target in colliders.AsSpan(0, count))
            {
                if (target.gameObject == gameObject || // 自分自身を除外
                    !target.gameObject.TryGetComponent(out Status enemyStatus)) // EnemyStatusがない場合は除外
                    continue;

                enemyStatus.Damage(damage);
            }

            ArrayPool<Collider>.Shared.Return(colliders);
        }

        private void Attack()
        {
            var colliders = ArrayPool<Collider>.Shared.Rent(16);
            int count = Physics.OverlapSphereNonAlloc(transform.position, detectRadius, colliders);

            foreach (Collider col in colliders.AsSpan(0, count))
            {
                if (!col.gameObject.CompareTag(Tag.Player))
                    continue;

                // プレイヤーに向けてのベクトル
                Vector3 targetDir = (col.transform.position - transform.position).normalized;
                
                // ランダムに拡散
                targetDir = Quaternion.AngleAxis(Random.Range(-angleRange,angleRange), Vector3.forward) * targetDir;
                
                // 発射
                Bullet bullet = Instantiate(bulletPrefab, transform.position + targetDir, Quaternion.identity).GetComponent<Bullet>();
                bullet.Shoot(targetDir * shootPower);
            }

            ArrayPool<Collider>.Shared.Return(colliders);
        }
    }
}