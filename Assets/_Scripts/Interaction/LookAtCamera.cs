using System;
using UnityEngine;

namespace _Scripts.Interaction
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;

        [Header("Rotation Offsets")]
        [SerializeField] private float offsetX = 0f;
        [SerializeField] private float offsetY = 0f;
        [SerializeField] private float offsetZ = 0f;

        private void Start()
        {
            if(!cameraTransform && Camera.main) cameraTransform = Camera.main?.transform;
        }

        private void Update()
        {
            if (!cameraTransform) return;
            
            transform.LookAt(cameraTransform);
            transform.Rotate(offsetX, offsetY, offsetZ);
        }
    }
}
