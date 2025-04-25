using System;
using UnityEngine;

namespace Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Rigidbody target;
        [SerializeField] private float damping;

        private Vector3 offset;

        private void Start()
        {
            offset = transform.position - target.position;
        }

        private void FixedUpdate()
        {
            Vector3 position = transform.position;
            position.x = Mathf.Lerp(transform.position.x, target.position.x + offset.x, damping);
            transform.position = position;
        }
    }
}