using System;
using System.Buffers;
using DG.Tweening;
using Player;
using UnityEngine;

namespace Grenade
{
    public class ScaleObject : MonoBehaviour
    {
        [SerializeField] private float minSize = 1f;
        [SerializeField] private float maxSize = 10000f;
        [SerializeField] private float impactPower;
        [SerializeField] private float impactRadius = 1.5f;
        [SerializeField] private float scalePower_Y = 1.5f;

        public void Scale(float size, float duration, bool charged)
        {
            float scaleX = transform.localScale.x;
            float nextScale = Mathf.Clamp(scaleX + size, minSize, maxSize);

            DOTween.To(() => scaleX,
                value =>
                {
                    scaleX = value;
                    ScaleAround(transform.localPosition - new Vector3(0f, 0.2f, 0f), new Vector3(scaleX, scaleX, scaleX));
                },
                nextScale,
                0.4f).SetEase(Ease.OutBack, 3f);

            if (charged)
            {
                ApplyImpact();
            }
        }

        private void ApplyImpact()
        {
            // 周囲にあるオブジェクトを取得
            var colliders = ArrayPool<Collider>.Shared.Rent(16);
            int count = Physics.OverlapSphereNonAlloc(transform.position, transform.localScale.x + impactRadius, colliders);

            // インパクトを与える
            foreach (Collider touchCollider in colliders.AsSpan(0, count))
            {
                if (!touchCollider.TryGetComponent(out Rigidbody rig))
                    continue;
                
                Vector2 direction;
                // cubeの上にいる場合は上方向の力だけ加える
                float topY = transform.position.y + transform.localScale.y / 2f;    // cubeの上面の高さ取得
                if (rig.transform.position.y >= topY)
                {
                    direction = Vector2.up;
                }
                else
                {
                    direction = (rig.transform.position - transform.position).normalized;
                }
              
                Vector2 force = new Vector2(direction.x * impactPower, direction.y * impactPower * scalePower_Y);

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


        private void ScaleAround(Vector3 pivot, Vector3 newScale)
        {
            Vector3 targetPos = transform.localPosition;
            Vector3 diff = targetPos - pivot;
            float relativeScale = newScale.x / transform.localScale.x;

            Vector3 resultPos = pivot + diff * relativeScale;
            transform.localScale = newScale;
            transform.localPosition = resultPos;
        }
    }
}