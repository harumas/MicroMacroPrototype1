using Constants;
using Enemy;
using UnityEngine;

namespace Cross
{
    public class ScaleBullet : MonoBehaviour
    {
        [SerializeField] private float scaleSize;
        [SerializeField] private float scaleDuration;

        private bool isCharged;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(Tag.Ground))
            {
                Destroy(gameObject);
                return;
            }

            if (other.gameObject.TryGetComponent(out ScaleObject scaleObject))
            {
                scaleSize = isCharged ? scaleSize * 3 : scaleSize;

                scaleObject.Scale(scaleSize, scaleDuration, isCharged && scaleSize > 0);
                Destroy(gameObject);
            }

            if (other.gameObject.TryGetComponent(out Status status) && !other.gameObject.CompareTag(Tag.Player))
            {
                if (scaleSize > 0)
                {
                    status.Damage(10f);
                }

                Destroy(gameObject);
            }
        }

        public void SetCharged(bool isCharged)
        {
            this.isCharged = isCharged;

            if (isCharged)
            {
                transform.localScale = Vector3.one;
            }
        }
    }
}