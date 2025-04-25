using UnityEngine;

namespace ScaleShooter
{
    public class ScaleBullet : MonoBehaviour
    {
        [SerializeField] private float scaleSize;
        [SerializeField] private float scaleDuration;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out ScalePivot scalePivot))
                return;

            scalePivot.Hit(new ScaleInfo(scaleSize, scaleDuration));
            Destroy(gameObject);
        }
    }
}