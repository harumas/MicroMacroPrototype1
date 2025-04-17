using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyStatus : MonoBehaviour
    {
        [SerializeField] private float hp;
        [SerializeField] private float maxHp = 100f;

        public event Action OnDeath;
        public event Action OnDamage;

        private void Start()
        {
            hp = maxHp;
        }

        public void Damage(float damage)
        {
            hp -= damage;
            hp = Mathf.Clamp(hp, 0, maxHp);

            if (hp == 0)
            {
                OnDeath?.Invoke();
            }
            else
            {
                OnDamage?.Invoke();
            }
        }
        
        public void Kill()
        {
            hp = 0;
            OnDeath?.Invoke();
        }

        public bool IsDeath()
        {
            return hp <= 0;
        }
    }
}