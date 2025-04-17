using System;
using System.Collections;
using Enemy;
using Gimmick;
using UnityEngine;

public class DarkSphere : MonoBehaviour
{
    [SerializeField] private EnemyStatus enemyStatus;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private ObjectActivator activator;
    [SerializeField] private int spawnCount;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float firstDelay;
    [SerializeField] private float normalSpawnInterval;

    private void Start()
    {
        enemyStatus.OnDamage += () => StartCoroutine(Spawn());
        StartCoroutine(NormalSpawn());
    }

    private IEnumerator NormalSpawn()
    {
        yield return new WaitForSeconds(firstDelay);

        while (true)
        {
            if (activator.IsVisible())
            {
                Instantiate(enemyPrefab, transform.position + Vector3.down, Quaternion.identity);
            }

            yield return new WaitForSeconds(normalSpawnInterval);
        }
    }

    private IEnumerator Spawn()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Instantiate(enemyPrefab, transform.position + Vector3.down, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}