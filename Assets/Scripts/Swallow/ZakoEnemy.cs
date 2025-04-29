using System;
using System.Buffers;
using System.Threading.Tasks;
using Constants;
using Enemy;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

namespace Swallow
{
    public class ZakoEnemy : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Status myStatus;
        [SerializeField] private VisualEffect explosionEffect;
        [SerializeField] private Collider collider;
        [SerializeField] private float distance;

        private Transform playerTransform;
        private bool isAlive = true;

        private void Start()
        {
            playerTransform = GameObject.FindWithTag("Player").transform;
            myStatus.OnDeath += async () =>
            {
                isAlive = false;
                agent.isStopped = true;
                explosionEffect.Play();
                collider.enabled = false;

                await Task.Delay(2000, destroyCancellationToken);

                Destroy(gameObject);
            };
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position,playerTransform.position) > distance)
                return;
            
            agent.SetDestination(playerTransform.position);
        }

        private void OnCollisionStay(Collision other)
        {
            if (other.GetContact(0).normal == Vector3.up)
                return;
            
            if (other.gameObject.CompareTag(Tag.Player) && isAlive)
            {
                if (other.gameObject.TryGetComponent(out Status status))
                {
                    status.Damage(20);
                }
            }
        }
    }
}