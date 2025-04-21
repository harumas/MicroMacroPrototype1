using System;
using Constants;
using Enemy;
using UnityEngine;

namespace Gimmick
{
    public class DangerZone : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(Tag.Player) && other.gameObject.TryGetComponent(out Status status))
            {
                status.Damage(1f);
            }
        }
    }
}
