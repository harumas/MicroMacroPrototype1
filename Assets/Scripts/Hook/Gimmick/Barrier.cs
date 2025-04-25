using System;
using Enemy;
using Player;
using UnityEngine;

namespace Gimmick
{
    public class Barrier : MonoBehaviour
    {
        [SerializeField] private Rigidbody rig;
        [SerializeField] private Status status;
        [SerializeField] private Renderer meshRenderer;
        [SerializeField] private CatchableObject catchableObject;

        private void Start()
        {
            catchableObject.OnRelease += OnRelease;
        }

        private void OnRelease(float scale)
        {
            rig.isKinematic = true;
            gameObject.layer = 0;
        }
    }
}