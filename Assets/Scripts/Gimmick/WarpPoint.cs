using System;
using Constants;
using UnityEngine;

namespace Gimmick
{
    public class WarpPoint : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        public event Action<Transform> OnTouch;

        public void Warp(Transform transform)
        {
            transform.position = this.transform.position + offset;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag(Tag.Player))
                return;
            
            OnTouch?.Invoke(other.transform);
        }
    }
}