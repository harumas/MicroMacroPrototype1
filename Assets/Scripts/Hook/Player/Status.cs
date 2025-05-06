using System;
using DG.Tweening;
using UnityEngine;

namespace Enemy
{
    public class Status : MonoBehaviour
    {
        [SerializeField] private float hp;
        [SerializeField] private float maxHp = 100f;
        [SerializeField] private float noDamageTime;

        public float Hp => hp;
        public float MaxHp => maxHp;
        private float lastDamageTime;

        public event Action OnDeath;
        public event Action OnDamage;

        private void Start()
        {
            hp = maxHp;
        }

        public void Damage(float damage)
        {
            if (Time.time - lastDamageTime < noDamageTime)
            {
                return;
            }

            hp -= damage;
            hp = Mathf.Clamp(hp, 0, maxHp);
            lastDamageTime = Time.time;

            if (hp == 0)
            {
                OnDeath?.Invoke();
            }
            else
            {
                OnDamage?.Invoke();
            }
        }

        public void SetMaxHp(float value)
        {
            maxHp = value;
            hp = value;
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