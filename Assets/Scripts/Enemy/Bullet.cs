using System;
using System.Buffers;
using Constants;
using Player;
using UnityEngine;

namespace Enemy
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float damage;
        [SerializeField] private CatchableObject catchableObject;
        [SerializeField] private Rigidbody rb;

        private bool isCatch;

        private void Start()
        {
            catchableObject.OnCatch += OnCatch;
            catchableObject.OnRelease += OnRelease;
        }

        private void OnRelease(float releasedScale)
        {
            ExplodeDamage(releasedScale);

            Destroy(gameObject, 0.5f);
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

        public void Shoot(Vector2 velocity)
        {
            rb.AddForce(velocity, ForceMode.Impulse);
        }

        private void OnCatch()
        {
            isCatch = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isCatch)
                return;

            if (!other.gameObject.CompareTag(Tag.Enemy) && other.gameObject.TryGetComponent(out Status status))
            {
                status.Damage(damage);
                Destroy(gameObject);
            }

            if (other.gameObject.CompareTag(Tag.Ground))
            {
                Destroy(gameObject);
            }
        }
    }
}