using System;
using UnityEngine;

namespace Mechanics
{
    public class RideConnector : MonoBehaviour
    {
        [SerializeField] private Rigidbody rig;
        private Vector3 prevPosition;
        private Vector3 difference;

        private void Start()
        {
            prevPosition = rig.position;
        }

        private void FixedUpdate()
        {
            difference = rig.position - prevPosition;
            prevPosition = rig.position;
        }

        private void OnCollisionStay(Collision other)
        {
            if (!other.gameObject.TryGetComponent(out Rigidbody rb))
                return;
            
            if (Vector3.Dot(-other.GetContact(0).normal, Vector3.up) > 0.9f)
            {
                // Rigidbodyの位置を更新
                rb.position += difference;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (!other.gameObject.TryGetComponent(out Rigidbody rb))
                return;

            rb.isKinematic = false;
        }
    }
}