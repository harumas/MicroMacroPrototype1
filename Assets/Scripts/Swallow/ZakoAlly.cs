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
    public class ZakoAlly : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Status myStatus;
        [SerializeField] private VisualEffect explosionEffect;
        [SerializeField] private float damageInterval = 1f;
        [SerializeField] private float damageRadius = 2f;
        [SerializeField] private float damage = 20f;
        [SerializeField] private Collider collider;

        private bool isAlive = true;
        private Transform currentTarget;

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

            DamageLoop();
        }

        private void Update()
        {
            if (currentTarget != null)
            {
                agent.SetDestination(currentTarget.transform.position);
            }
        }

        private async void DamageLoop()
        {
            while (isAlive)
            {
                Collider[] colliders = ArrayPool<Collider>.Shared.Rent(16);
                int count = Physics.OverlapSphereNonAlloc(transform.position, transform.localScale.x + damageRadius, colliders);

                for (var i = 0; i < count; i++)
                {
                    var col = colliders[i];
                    if (!col.gameObject.CompareTag(Tag.Enemy))
                        continue;

                    col.GetComponent<Status>().Damage(damage);
                    break;
                }

                ArrayPool<Collider>.Shared.Return(colliders);

                await Task.Delay(TimeSpan.FromSeconds(damageInterval), destroyCancellationToken);

                Vector3 diff = currentTarget.position - transform.position;

                if (Vector3.Angle(transform.forward, diff) > 20f)
                {
                    await Task.Delay(1, destroyCancellationToken);
                }
            }
        }

        private async void DetectEnemies()
        {
            while (isAlive)
            {
                Collider[] colliders = ArrayPool<Collider>.Shared.Rent(16);
                int count = Physics.OverlapSphereNonAlloc(transform.position, transform.localScale.x + damageRadius, colliders);

                for (var i = 0; i < count; i++)
                {
                    var col = colliders[i];
                    if (!col.gameObject.CompareTag(Tag.Enemy))
                        continue;

                    col.GetComponent<Status>().Damage(damage);
                    break;
                }

                ArrayPool<Collider>.Shared.Return(colliders);

                await Task.Delay(TimeSpan.FromSeconds(damageInterval), destroyCancellationToken);

                Vector3 diff = currentTarget.position - transform.position;

                if (Vector3.Angle(transform.forward, diff) > 20f)
                {
                    await Task.Delay(1, destroyCancellationToken);
                }
            }
        }
    }
}