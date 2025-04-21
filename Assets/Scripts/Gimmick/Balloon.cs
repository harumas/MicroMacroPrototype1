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

        private float antiGravityPower;
        private Vector3 defaultScale;

        private void Start()
        {
            catchableObject.OnRelease += OnRelease;
            defaultScale = transform.localScale;
        }

        private void FixedUpdate()
        {
            rig.AddForce(Vector3.up * antiGravityPower);

            transform.localScale = Vector3.Lerp(transform.localScale, defaultScale, unscaleSpeed * Time.fixedDeltaTime);
            antiGravityPower = transform.localScale.x * antiGravityScale;
        }

        private void OnRelease(float scale)
        {
            antiGravityPower = scale * antiGravityScale;
        }
    }
}