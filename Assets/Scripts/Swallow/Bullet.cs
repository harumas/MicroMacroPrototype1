using System;
using Constants;
using Enemy;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Tag.Player) &&
            collision.gameObject.TryGetComponent(out Status status))
        {
            status.Damage(10f);
        }
        
        Destroy(gameObject);
    }
}