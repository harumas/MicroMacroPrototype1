using System;
using System.Collections;
using Enemy;
using Gimmick;
using UnityEngine;
using UnityEngine.Serialization;

public class DarkSphere : MonoBehaviour
{
    [FormerlySerializedAs("enemyStatus")] [SerializeField] private Status status;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private ObjectActivator activator;
    [SerializeField] private int spawnCount;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float firstDelay;
    [SerializeField] private float normalSpawnInterval;

    private void Start()
    {
        status.OnDamage += () => StartCoroutine(Spawn());
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