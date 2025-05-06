using System;
using Enemy;
using UnityEngine;

namespace Takanashi
{
    public class PlayerAttacker : MonoBehaviour
    {
        [SerializeField] private float attackPower = 10f;
        [SerializeField] private PlayerMovement playerMovement;

        private void OnCollisionEnter(Collision other)
        {
            Status status = null;
            if (playerMovement.IsMoving && !other.gameObject.TryGetComponent(out status))
                return;

            
            status?.Damage(attackPower);
        }

    }
}