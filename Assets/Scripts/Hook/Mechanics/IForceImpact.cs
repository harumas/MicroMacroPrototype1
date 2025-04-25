using UnityEngine;

namespace Player
{
    public interface IForceImpact
    {
        void AddForce(Vector2 force, float freezeTime);
    }
}