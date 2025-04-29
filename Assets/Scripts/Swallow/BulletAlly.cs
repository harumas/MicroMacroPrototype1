using System;
using Constants;
using Enemy;
using UnityEngine;

namespace Swallow
{
    public class BulletAlly : MonoBehaviour, IAllyItem
    {
        [SerializeField] private Rigidbody rig;
        [SerializeField] private Collider col;
        [SerializeField] private float damage;

        public void Initialize()
        {
            col.enabled = false;
        }

        public void Use(bool isBig, Vector3 direction)
        {
            transform.localScale = Vector3.one * (isBig ? 4f : 1f);
            col.enabled = true;
            rig.isKinematic = false;
            rig.useGravity = false;
            rig.AddForce(direction * (isBig ? 3f : 9.5f), ForceMode.VelocityChange);
        }

        public void Disable() { }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag(Tag.Player) && other.gameObject.TryGetComponent(out Status status))
            {
                status.Damage(damage);
                Destroy(gameObject);
            }
        }
    }
}