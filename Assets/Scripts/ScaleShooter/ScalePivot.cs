using System;
using UnityEngine;

namespace ScaleShooter
{
    public class ScalePivot : MonoBehaviour
    {
        public event Action<Vector3, ScaleInfo> OnHit;

        public void Hit(ScaleInfo scaleInfo)
        {
            Vector3 pivot = transform.position;
            OnHit?.Invoke(pivot, scaleInfo);
        }
    }
}