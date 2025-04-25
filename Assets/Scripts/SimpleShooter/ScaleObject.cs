using DG.Tweening;
using UnityEngine;

public class ScaleObject : MonoBehaviour
{
    [SerializeField] private float minSize = 1f;
    [SerializeField] private float maxSize = 10000f;

    public void Scale(float size, float duration)
    {
        float scaleX = transform.localScale.x;
        float nextScale = Mathf.Clamp(scaleX + size, minSize, maxSize);

        transform.DOScale(nextScale, duration).SetEase(Ease.OutBack, 3f);
    }
}