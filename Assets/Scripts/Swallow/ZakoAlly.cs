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
    public class ZakoAlly : MonoBehaviour, IAllyItem
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Rigidbody rig;
        [SerializeField] private Status myStatus;
        [SerializeField] private VisualEffect explosionEffect;
        [SerializeField] private Collider collider;
        [SerializeField] private float detectRadius;

        private bool isAlive;
        private bool isEnabled;
        private Transform enemy;

        private void Start()
        {
            myStatus.OnDeath += async () =>
            {
                isAlive = false;
                agent.isStopped = true;
                explosionEffect.Play();
                collider.enabled = false;

                await Task.Delay(2000, destroyCancellationToken);

                Destroy(gameObject);
            };

            Disable();
        }

        private void Update()
        {
            if (agent.enabled && enemy != null)
            {
                agent.SetDestination(enemy.position);
            }

            DetectEnemies();
        }
        
        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.CompareTag(Tag.Enemy) && isAlive)
            {
                if (other.gameObject.TryGetComponent(out Status status))
                {
                    status.Damage(20);
                }
            }
        }

        private void DetectEnemies()
        {
            if (enemy != null)
                return;

            Collider[] colliders = ArrayPool<Collider>.Shared.Rent(16);
            int count = Physics.OverlapSphereNonAlloc(transform.position, detectRadius, colliders);

            if (count == 0)
                return;

            for (var i = 0; i < count; i++)
            {
                if (!colliders[i].CompareTag(Tag.Enemy))
                    continue;

                enemy = colliders[i].transform;
                break;
            }

            ArrayPool<Collider>.Shared.Return(colliders);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!isEnabled && other.gameObject.CompareTag(Tag.Ground))
            {
                Enable();
            }
        }

        public void Enable()
        {
            agent.enabled = true;
            isAlive = true;
            isEnabled = true;
            rig.isKinematic = true;
            rig.useGravity = false;
        }

        public void Initialize()
        {
            transform.localScale = Vector3.one * 0.5f;
        }

        public void Use(bool isBig, Vector3 direction)
        {
            if (isBig)
            {
                agent.speed = 1f;
            }
            
            transform.localScale = Vector3.one * (isBig ? 2f : 1f);
            collider.enabled = true;
            rig.isKinematic = false;
            rig.useGravity = true;
            rig.AddForce(direction * 12f + Vector3.up * 12f, ForceMode.VelocityChange);
        }

        public void Disable()
        {
            agent.enabled = false;
            collider.enabled = false;
            rig.isKinematic = true;
        }
    }
}