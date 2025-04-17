using DG.Tweening;
using UnityEngine;

namespace Enemy
{
    public class LightSphere : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private EnemyStatus enemyStatus;

        private void Start()
        {
            enemyStatus.OnDeath += OnDeath;
        }

        private void OnDeath()
        {
            Material material = GetComponent<Renderer>().material;
            material.DOFade(0f, 0.5f).OnComplete(() => { Destroy(gameObject); });
        }

        private void FixedUpdate()
        {
            transform.rotation *= Quaternion.Euler(0f, 0f, rotationSpeed);
        }
    }
}