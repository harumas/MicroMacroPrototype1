using System;
using Unity.Cinemachine;
using UnityEngine;

namespace TakanashiExtended
{
    public class CameraBlender : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera playerCamera;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerCamera.Priority = -1;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerCamera.Priority = 100;
            }
        }
    }
}