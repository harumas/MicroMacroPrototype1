using System;
using System.Buffers;
using System.Threading.Tasks;
using Constants;
using DG.Tweening;
using Enemy;
using Player;
using UnityEngine;

namespace Cross
{
    public class Zako : MonoBehaviour, IForceImpact
    {
        [SerializeField] private float damage;
        [SerializeField] private float moveSpeed;
        [SerializeField] private bool inverseDirection;
        [SerializeField] private Rigidbody rig;
        [SerializeField] private Status status;

        private float freezeTime;
        private int direction = -1;

        private void Start()
        {
            status.OnDeath += () =>
            {
                transform.DOScale(0f, 0.3f).SetEase(Ease.OutBack, 3f).OnComplete(() => Destroy(gameObject));
            };
        }

        private void FixedUpdate()
        {
            if (rig.isKinematic || !IsGround())
                return;

            if (freezeTime > 0f)
            {
                freezeTime -= Time.fixedDeltaTime;
                freezeTime = Mathf.Max(0f, freezeTime);
                return;
            }

            rig.linearVelocity = transform.right * (moveSpeed * direction);

            if (inverseDirection && IsWall())
            {
                direction = -direction;
            }
        }

        private bool IsGround()
        {
            return Physics.Raycast(transform.position, Vector3.down, 1.05f);
        }

        private bool IsWall()
        {
            return Physics.Raycast(transform.position, transform.right * direction, 1.05f);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag(Tag.Player))
                return;

            Status playerStatus = other.gameObject.GetComponent<Status>();
            playerStatus.Damage(damage);
        }

        public void AddForce(Vector2 force, float freezeTime)
        {
            this.freezeTime = freezeTime;
            rig.linearVelocity = Vector3.zero;
            rig.AddForce(force, ForceMode.Impulse);
        }
    }
}