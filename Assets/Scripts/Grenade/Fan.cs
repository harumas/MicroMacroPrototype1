using System.Collections.Generic;
using UnityEngine;

namespace Grenade
{
    public class Fan : MonoBehaviour
    {
        private static readonly Dictionary<Direction, Vector3> DirectionToVector = new()
        {
            { Direction.Up, Vector3.up },
            { Direction.Right, Vector3.right },
            { Direction.Left, Vector3.left },
            { Direction.Down, Vector3.down },
            { Direction.Forward, Vector3.forward },
            { Direction.Back, Vector3.back }
        };

        [SerializeField]
        private float liftForce = 10f;

        [SerializeField]
        private Direction direction;

        [SerializeField] private float speedDawn =0.5f;
        private readonly Dictionary<Collider, Rigidbody> cachedRigidbodies = new();
        private Vector3 windDirection = Vector3.up;

        private void Awake()
        {
            windDirection = DirectionToVector[direction];
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                var rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    cachedRigidbodies[other] = rb;
                    rb.useGravity = false; // エリア内では重力をオフに
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (cachedRigidbodies.TryGetValue(other, out var rb))
            {
                rb.useGravity = true; // エリアを出たら重力を再びオンに
                cachedRigidbodies.Remove(other);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (cachedRigidbodies.TryGetValue(other, out var rb))
            {
                var lift = windDirection * liftForce;
                rb.AddForce(lift, ForceMode.Acceleration);
                rb.linearVelocity *= speedDawn;
            }
        }
        
        private void OnValidate()
        {
            liftForce = Mathf.Max(0f, liftForce);
            if (windDirection != Vector3.zero)
                windDirection.Normalize();
        }

        private enum Direction
        {
            [InspectorName("上(Y)")] Up,
            [InspectorName("右(X)")] Right,
            [InspectorName("左(-X)")] Left,
            [InspectorName("下(-Y)")] Down,
            [InspectorName("正面(Z)")] Forward,
            [InspectorName("後ろ(-Z)")] Back
        }
    }
}