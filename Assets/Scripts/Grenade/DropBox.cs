using Player;
using UnityEngine;

namespace Grenade
{
    public class DropBox : MonoBehaviour, IForceImpact
    {
        [SerializeField] private Rigidbody rig;
        private float freezeTime;
        public void AddForce(Vector2 force, float freezeTime)
        {
            this.freezeTime = freezeTime;
            rig.linearVelocity = Vector3.zero;
            rig.AddForce(force, ForceMode.Impulse);
        }
    }
}

