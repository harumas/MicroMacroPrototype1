using System;
using DG.Tweening;
using UnityEngine;

namespace ScaleShooter
{
    public class ScalableObject : MonoBehaviour
    {
        [SerializeField] private float minSize = 1f;
        [SerializeField] private float maxSize = 10000f;
        [SerializeField] private ScalePivot[] pivots;

        private Tween currentTween;

        private void Start()
        {
            foreach (ScalePivot pivot in pivots)
            {
                pivot.OnHit += Scale;
            }
        }

        private void Scale(Vector3 pivot, ScaleInfo scaleInfo)
        {
            float scaleX = transform.localScale.x;
            float nextScale = Mathf.Clamp(scaleX + scaleInfo.Size, minSize, maxSize);

            currentTween?.Kill();
            currentTween = DOTween.To(() => scaleX,
                value =>
                {
                    scaleX = value;
                    ScaleAround(pivot, new Vector3(scaleX, scaleX, scaleX));
                },
                nextScale,
                scaleInfo.Duration).SetEase(Ease.OutBack, 3f);
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