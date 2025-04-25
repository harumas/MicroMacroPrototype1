using System;
using UnityEngine;

namespace Gimmick
{
    [Serializable]
    public struct WarpPointSet
    {
        public WarpPoint pointA;
        public WarpPoint pointB;
    }

    public class WarpPointManager : MonoBehaviour
    {
        [SerializeField] private WarpPointSet[] warpPoints;

        private void Start()
        {
            foreach (WarpPointSet point in warpPoints)
            {
                point.pointA.OnTouch += point.pointB.Warp;
                point.pointB.OnTouch += point.pointA.Warp;
            }
        }
    }
}