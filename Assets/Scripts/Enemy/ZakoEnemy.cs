using System;
using System.Buffers;
using System.Threading.Tasks;
using Constants;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace Enemy
{
    public class ZakoEnemy : MonoBehaviour, IForceImpact
    {
        [SerializeField] private float damage;
        [SerializeField] private float moveSpeed;
        [SerializeField] private Rigidbody rig;
        [SerializeField] private Collider col;
        [SerializeField] private Renderer meshRenderer;
        [SerializeField] private CatchableObject catchableObject;
        [SerializeField] private Status status;

        private bool isCatch;
        private float freezeTime;

        private void Start()
        {
            catchableObject.OnCatch += OnCatch;
            catchableObject.OnRelease += OnRelease;

            status.OnDeath += () =>
            {
                if (catchableObject.CurrentState != CatchableObject.State.PreRelease)
                    return;

                ObjectCatcher objectCatcher = GameObject.FindWithTag("Player").GetComponent<ObjectCatcher>();
                catchableObject.Release(objectCatcher.CalculateScale());
            };
        }

        private void OnCatch()
        {
            isCatch = true;
        }

        private async void OnRelease(float scale)
        {
            rig.isKinematic = true;
            col.enabled = false;
            gameObject.layer = 0;

            status.Kill();

            await Task.Delay(300, destroyCancellationToken);

            if (catchableObject.CurrentState == CatchableObject.State.Release)
            {
                ExplodeDamage(scale);
            }

            meshRenderer.material.DOFade(0f, 0.5f)
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }

        private void ExplodeDamage(float releasedScale)
        {
            // 周囲にあるオブジェクトを取得
            var colliders = ArrayPool<Collider>.Shared.Rent(16);
            int count = Physics.OverlapSphereNonAlloc(transform.position, releasedScale * 0.5f + 0.5f, colliders);

            foreach (Collider target in colliders.AsSpan(0, count))
            {
                if (target.gameObject == gameObject || // 自分自身を除外
                    !target.gameObject.TryGetComponent(out Status enemyStatus)) // 敵ではない場合は除外
                    continue;

                enemyStatus.Damage(damage);
            }

            ArrayPool<Collider>.Shared.Return(colliders);
        }

        private void FixedUpdate()
        {
            if (rig.isKinematic || isCatch || !IsGround())
                return;

            if (freezeTime > 0f)
            {
                freezeTime -= Time.fixedDeltaTime;
                freezeTime = Mathf.Max(0f, freezeTime);
                return;
            }

            rig.linearVelocity = transform.right * moveSpeed;
        }

        private bool IsGround()
        {
            return Physics.Raycast(transform.position, Vector3.down, 1.05f);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag(Tag.Player) || isCatch)
                return;

            Status playerStatus = other.gameObject.GetComponent<Status>();
            playerStatus.Damage(damage);
        }

        public void AddForce(Vector2 force, float freezeTime)
        {
            this.freezeTime = freezeTime;
            rig.linearVelocity = Vector3.zero;
            rig.AddForce(force, ForceMode.Impulse);
        }
    }
}