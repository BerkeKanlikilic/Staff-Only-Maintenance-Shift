using System;
using _Scripts.Game;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerCameraController : NetworkBehaviour
    {
        [SerializeField] private float cameraYOffset = 1.4f;
        [SerializeField] private float lookSensitivity = 2f;
        [SerializeField] private float maxLookAngle = 80f;

        private Transform _cameraTransform;
        private PlayerInputManager _input;
        private float _xRotation = 0f;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            
            _input = GetComponent<PlayerInputManager>();
            
            _cameraTransform = Camera.main.transform;
            _cameraTransform.SetParent(transform);
            _cameraTransform.localPosition = new Vector3(0, cameraYOffset, 0);
            _cameraTransform.localRotation = Quaternion.identity;
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            if(!_cameraTransform)
                Debug.LogError("Camera.main was null. Make sure your player prefab has a tagged MainCamera.");
        }

        private void LateUpdate()
        {
            if (GameManager.GameState.IsGameFrozen) return;
            if (!_cameraTransform || !_input || !_input.CanProcessInput()) return;
            
            Vector2 lookInput = _input.LookInput;
            float mouseX = lookInput.x * (lookSensitivity / 10f);
            float mouseY = lookInput.y * (lookSensitivity / 10f);

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -maxLookAngle, maxLookAngle);
            
            _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
    }
}
