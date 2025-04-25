using System;
using System.Threading.Tasks;
using Cross;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class StrongMan : MonoBehaviour, IForceImpact
{
    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private Vector2 spawnOffset;
    [SerializeField] private float holdTime;
    [SerializeField] private float distance;
    [SerializeField] private float throwDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float deathScale;
    [SerializeField] private Rigidbody rig;

    private GameObject currentStone;
    private Transform player;
    private float freezeTime;
    private bool isDeath;
    private bool isEnabled;

    private async void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        while (!destroyCancellationToken.IsCancellationRequested)
        {
            currentStone = Instantiate(stonePrefab, transform.position + (Vector3)spawnOffset, Quaternion.identity);
            currentStone.transform.SetParent(transform);

            Rigidbody rig = currentStone.GetComponent<Rigidbody>();
            rig.isKinematic = true;

            await Task.Delay(TimeSpan.FromSeconds(holdTime), destroyCancellationToken);
            if (isDeath)
            {
                return;
            }

            if (!isEnabled)
            {
                Destroy(currentStone);
            }
            else
            {
                if (rig != null)
                {
                    rig.isKinematic = false;

                    currentStone.transform.SetParent(null);
                    rig.AddForce(CalculateLaunchVelocity(currentStone.transform.position, player.position, 45), ForceMode.Impulse);
                    currentStone = null;
                }
            }


            await Task.Delay(TimeSpan.FromSeconds(0.5f), destroyCancellationToken);
            if (isDeath)
            {
                return;
            }
        }
    }

    public static Vector3 CalculateLaunchVelocity(Vector3 startPoint, Vector3 targetPoint, float launchAngle)
    {
        Vector3 direction = targetPoint - startPoint;
        Vector3 directionXZ = new Vector3(direction.x, 0f, direction.z);

        float distance = directionXZ.magnitude;
        float heightDifference = direction.y;
        float radianAngle = launchAngle * Mathf.Deg2Rad;

        float velocity = Mathf.Sqrt((Physics.gravity.y * distance * distance) /
                                    (2 * (heightDifference - distance * Mathf.Tan(radianAngle)) * Mathf.Pow(Mathf.Cos(radianAngle), 2)));

        Vector3 launchVelocity = directionXZ.normalized * velocity * Mathf.Cos(radianAngle);
        launchVelocity.y = velocity * Mathf.Sin(radianAngle);

        return launchVelocity;
    }

    private void FixedUpdate()
    {
        if (rig.isKinematic || !IsGround() || isDeath)
            return;

        if (freezeTime > 0f)
        {
            freezeTime -= Time.fixedDeltaTime;
            freezeTime = Mathf.Max(0f, freezeTime);
            return;
        }

        Vector3 diff = player.position - transform.position;

        transform.rotation = Quaternion.LookRotation(Vector3.forward * -Mathf.Sign(diff.x), Vector3.up);

        float targetDir = Mathf.Sign(transform.position.x - player.position.x);
        float diffVec = player.position.x + (targetDir * distance) - transform.position.x;

        if (Mathf.Abs(diffVec) < 0.1f)
        {
            return;
        }

        rig.linearVelocity = Vector3.right * (diffVec * moveSpeed);
    }

    private void Update()
    {
        isEnabled = Vector3.Distance(transform.position, player.position) <= throwDistance;

        if (currentStone == null)
            return;

        if (currentStone.transform.localScale.x >= deathScale)
        {
            Kill();
            isDeath = true;
        }
    }

    private void Kill()
    {
        currentStone.GetComponent<Rigidbody>().constraints = ~RigidbodyConstraints.FreezePositionY;
        currentStone.GetComponent<Stone>().DoDestroy = true;
        currentStone.transform.DOLocalMoveY(0f, 0.3f).OnComplete(() =>
        {
            Tween shake = null;

            shake = currentStone.transform.DOShakePosition(0.5f, 0.5f).OnComplete(() =>
            {
                shake.Kill();
                if (gameObject != null)
                {
                    Destroy(currentStone);
                    Destroy(gameObject);
                }
            });
        });
    }

    private bool IsGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.05f);
    }

    public void AddForce(Vector2 force, float freezeTime)
    {
        this.freezeTime = freezeTime;
        rig.linearVelocity = Vector3.zero;
        rig.AddForce(force, ForceMode.Impulse);
    }
}