using System;
using System.Buffers;
using Constants;
using DG.Tweening;
using UnityEngine;

namespace Player
{
    public class CatchableObject : MonoBehaviour
    {
        public enum State
        {
            PreRelease,
            Catch,
            Shoot,
            Release,
        }

        [SerializeField] private Rigidbody rb;
        [SerializeField] private Collider col;
        [SerializeField] private State state = State.PreRelease;
        [SerializeField] private float minScale = 0.5f;
        [SerializeField] private float maxScale = 2.0f;
        [SerializeField] private float scaleDuration = 0.3f;
        [SerializeField] private float impactRadius = 1.5f;
        [SerializeField] private float impactPower = 3f;
        [SerializeField] private float shootPower = 3f;

        private float defaultScale;
        private Tween currentTween;

        public State CurrentState => state;
        public event Action OnCatch;
        public event Action<Vector2> OnShoot;
        public event Action<float> OnRelease;

        private void Start()
        {
            defaultScale = transform.localScale.x;
        }

        public void Catch()
        {
            // 物理コンポーネントの無効化
            rb.isKinematic = true;
            col.enabled = false;

            // 小さくする
            currentTween?.Kill();
            currentTween = transform.DOScale(minScale, scaleDuration).SetEase(Ease.OutBack, 3f);

            state = State.Catch;
            OnCatch?.Invoke();
        }


        public void Shoot(Vector3 direction)
        {
            rb.isKinematic = false;
            col.enabled = true;

            Vector2 velocity = direction * shootPower;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(velocity, ForceMode.VelocityChange);

            state = State.Shoot;
            OnShoot?.Invoke(velocity);
        }

        public void Release(float scale)
        {
            // 物理コンポーネントの有効化
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            col.enabled = true;

            // 大きくする
            float realScale = (maxScale - defaultScale) * scale + defaultScale;
            currentTween?.Kill();
            currentTween = transform.DOScale(realScale, scaleDuration).SetEase(Ease.OutBack, 3f);

            ApplyImpact(scale);

            state = State.Release;
            OnRelease?.Invoke(realScale);
        }

        private void ApplyImpact(float scale)
        {
            // 周囲にあるオブジェクトを取得
            var colliders = ArrayPool<Collider>.Shared.Rent(16);
            int count = Physics.OverlapSphereNonAlloc(transform.position, minScale * impactRadius, colliders);

            // インパクトを与える
            foreach (Collider touchCollider in colliders.AsSpan(0, count))
            {
                if (!touchCollider.TryGetComponent(out Rigidbody rig))
                    continue;

                Vector2 direction = (rig.transform.position - transform.position).normalized;
                Vector2 force = direction * (impactPower * scale);

                if (touchCollider.TryGetComponent(out PlayerMovement playerMovement))
                {
                    playerMovement.AddExternalForce(force);
                }
                else if (touchCollider.TryGetComponent(out IForceImpact forceImpact))
                {
                    forceImpact.AddForce(force, 1f);
                }
            }

            ArrayPool<Collider>.Shared.Return(colliders);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, minScale * impactRadius);
        }

        public void Sync(Vector3 position)
        {
            transform.position = position;
        }

        private void OnCollisionEnter(Collision other)
        {
            // 勝手に起爆するのはナシ
            // if (state == State.Shoot && other.gameObject.CompareTag(Tag.Ground))
            // {
            //     ObjectCatcher objectCatcher = GameObject.FindWithTag("Player").GetComponent<ObjectCatcher>();
            //     Release(objectCatcher.CalculateScale());
            // }
        }
    }
}