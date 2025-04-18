using System;
using Constants;
using UnityEngine;

namespace Gimmick
{
    public class Coin : MonoBehaviour
    {
        private void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.CompareTag(Tag.Player))
            {
                Destroy(gameObject);
            }
        }
    }
}