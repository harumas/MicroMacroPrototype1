using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class LightSphere : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 1f;
        [FormerlySerializedAs("enemyStatus")] [SerializeField] private Status status;

        private void Start()
        {
            status.OnDeath += OnDeath;
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