using System;
using Constants;
using Enemy;
using Unity.VisualScripting;
using UnityEngine;

namespace Grenade
{
    public class ScaleGrenade : MonoBehaviour
    {
        [SerializeField] private float scaleSize;
        [SerializeField] private float scaleDuration;
        [SerializeField] private float deleteTime;

        private float time;
        private bool isStay;
        private void OnCollisionEnter(Collision other)
        {
            isStay = true;
            
            if (other.gameObject.TryGetComponent(out Grenade.ScaleObject scaleObject))
            {
                // チャージ無くす
                //scaleSize = isCharged ? scaleSize * 3 : scaleSize;

                scaleObject.Scale(scaleSize, scaleDuration,  scaleSize > 0);
                Destroy(gameObject);
            }

            if (other.gameObject.TryGetComponent(out Status status) && !other.gameObject.CompareTag(Tag.Player))
            {
                if (scaleSize > 0)
                {
                    status.Damage(10f);
                }

                Destroy(gameObject);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            isStay = false;
            time = 0f;
        }

        private void Update()
        {
            if (isStay)
            {
                time += Time.deltaTime;
                if (time >= deleteTime)
                    Destroy(this.gameObject);
            }
        }
    }
}