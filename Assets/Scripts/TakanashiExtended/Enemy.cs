using System;
using Enemy;
using UnityEngine;

namespace TakanashiExtended
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Status status;
        
        private void Start()
        {
            status.OnDeath += () => 
            {
                // Handle enemy death
                Destroy(gameObject);
            };
        }
    }
}