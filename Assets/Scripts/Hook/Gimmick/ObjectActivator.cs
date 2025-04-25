using UnityEngine;

namespace Gimmick
{
    public class ObjectActivator : MonoBehaviour
    {
        private Rigidbody rb;
        [SerializeField] private float deadDistance = 20f;
        [SerializeField] private float distance;
        private bool didVisible;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }

        private void Update()
        {
            distance = Mathf.Abs(Camera.main.transform.position.x - transform.position.x);

            if (!didVisible && rb != null && IsVisible())
            {
                rb.isKinematic = false;
                didVisible = true;
            }
        }

        public bool IsVisible()
        {
            return distance <= deadDistance;
        }
    }
}