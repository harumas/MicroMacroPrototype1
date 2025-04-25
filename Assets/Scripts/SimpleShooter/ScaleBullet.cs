using ScaleShooter;
using UnityEngine;

namespace SimpleShooter
{
    public class ScaleBullet : MonoBehaviour
    {
        [SerializeField] private float scaleSize;
        [SerializeField] private float scaleDuration;

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.TryGetComponent(out ScaleObject scaleObject))
                return;

            scaleObject.Scale(scaleSize, scaleDuration);
            Destroy(gameObject);
        }
    }
}