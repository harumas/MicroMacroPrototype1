using System;
using System.Collections;
using System.Threading.Tasks;
using Enemy;
using UnityEngine;
using UnityEngine.VFX;

public class ShootEnemy : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject prefab;
    [SerializeField] private VisualEffect explosionEffect;
    [SerializeField] private Status myStatus;
    [SerializeField] private Collider collider;
    [SerializeField] private float interval;
    [SerializeField] private float power;
    [SerializeField] private float detectRadius;

    private bool isAlive = true;

    private void Start()
    {
        StartCoroutine(ShootLoop());
        myStatus.OnDeath += async () =>
        {
            isAlive = false;
            explosionEffect.Play();
            collider.enabled = false;

            await Task.Delay(2000, destroyCancellationToken);

            Destroy(gameObject);
        };
    }

    private void Update()
    {
        Vector3 position = playerTransform.position;
        position.y = transform.position.y;

        transform.LookAt(position);
    }

    private IEnumerator ShootLoop()
    {
        while (isAlive)
        {
            GameObject obj = Instantiate(prefab, transform.position + transform.forward * 2f, Quaternion.identity);
            obj.GetComponent<Rigidbody>().AddForce(transform.forward * power, ForceMode.VelocityChange);

            yield return new WaitForSeconds(interval);
            yield return new WaitUntil(() => Vector3.Distance(transform.position, playerTransform.position) < detectRadius);
        }
    }
}