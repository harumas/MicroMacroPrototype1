using UnityEngine;
using System.Collections;

namespace Grenade
{
    public class Reflection : MonoBehaviour
    {
        [Header("反射時は力を〇倍する")]
        [SerializeField] private Vector3 scalePower;
        private Rigidbody _rb;
        private bool _wait;
        private int _bounceCou;
        private Vector3 _inDirection; 
        private void OnCollisionEnter(Collision other)
        {
            if (_wait)
                return;

            _bounceCou++;
            if (_bounceCou >= 5)
                Destroy(this);
            
            if (!other.gameObject.CompareTag("Wall"))
                return;
            
            if (_rb == null)
            {
                _rb = gameObject.GetComponent<Rigidbody>();
            }
            
            // 入射ベクトル (速度）
            _inDirection = _rb.linearVelocity;
            // 法線ベクトル 接地点から取得
            // Vector3 inNormal =  other.contacts[0].normal;
            Vector3 inNormal = other.contacts[0].normal;
        
            // 反射ベクトル（速度）
            Vector3 result = Vector3.Reflect(_inDirection, inNormal);
        
            _rb.linearVelocity = Vector3.zero;
        
            // 反射後の速度を反映
            _rb.linearVelocity = Vector3.Scale(result, scalePower);
          //  GameManager.instance.Directon = result.normalized;
          
            StartCoroutine("WaitAndReflective");
        }
        

        //OnCollisionが何度も起きないように待機時間入れてみる
        private IEnumerator WaitAndReflective()
        {
            _wait = true;
            yield return new WaitForSeconds(0.1f);
            _wait = false;
        }
    }
}