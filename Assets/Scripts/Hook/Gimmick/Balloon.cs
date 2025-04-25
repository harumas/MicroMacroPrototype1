using System;
using Enemy;
using Player;
using UnityEngine;

namespace Gimmick
{
    public class Balloon : MonoBehaviour
    {
        [SerializeField] private float antiGravityScale = 0.5f;
        [SerializeField] private float unscaleSpeed = 0.1f;
        [SerializeField] private CatchableObject catchableObject;
        [SerializeField] private Rigidbody rig;
        [SerializeField] private Transform lookTransform;

        private float antiGravityPower;
        private bool wasShoot;
        private Vector3 defaultScale;
        private Vector3 lookDirection;

        private void Start()
        {
            catchableObject.OnRelease += OnRelease;
            catchableObject.OnShoot += OnShoot;
            defaultScale = transform.localScale;
        }

        private void FixedUpdate()
        {
            rig.AddForce(lookDirection * antiGravityPower);

            transform.localScale = Vector3.Lerp(transform.localScale, defaultScale, unscaleSpeed * Time.fixedDeltaTime);
            antiGravityPower = transform.localScale.x * antiGravityScale;
        }

        private void OnShoot(Vector2 direction)
        {
            lookDirection = Vector3.up;
            wasShoot = true;
        }

        private void OnRelease(float scale)
        {
            antiGravityPower = scale * antiGravityScale;

            if (!wasShoot)
            {
                lookDirection = lookTransform.right;
            }

            wasShoot = false;
        }
    }
}