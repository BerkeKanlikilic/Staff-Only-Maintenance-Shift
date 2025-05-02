using System.Collections.Generic;
using _Scripts.UI;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace _Scripts.Player
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerController : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 7.5f;
        [SerializeField] private float sprintSpeed = 11.5f;
        [SerializeField] private float jumpForce = 1f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float groundCheckDistance = 0.2f;

        [Header("Look Settings")]
        [Range(1,10)] [SerializeField] private float lookSensitivity = 2f;
        [SerializeField] private float maxLookAngle = 80f;

        [Header("Camera Settings")]
        [SerializeField] private float cameraYOffset = 1.4f;

        [Header("References")]
        [SerializeField] private List<Renderer> playerRenderers;
        [SerializeField] private TMP_Text nameText;
    
        private Camera _playerCamera;
        private CharacterController _characterController;
        private UIManager _uiManager;
        private Vector3 _velocity;
        private float _verticalRotation = 0f;

        private PlayerInput _playerInput;
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private bool _isJumping;
        private bool _isSprinting;
        
        public static PlayerController Local { get; private set; }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (!IsOwner) return;

            _playerCamera.transform.SetParent(null);
            _playerCamera.transform.position = Vector3.zero;
            _playerCamera = null;
            
            Local = null;

            if (_playerInput != null) _playerInput.enabled = false;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            
            Local = this;
            _playerCamera = Camera.main;
            if (_playerCamera != null)
            {
                _playerCamera.transform.position = new Vector3(transform.position.x,
                    transform.position.y + cameraYOffset, transform.position.z);
                _playerCamera.transform.SetParent(transform);
            }

            SetShadowsOnly();
            nameText.enabled = false;

            _playerInput = GetComponent<PlayerInput>();
            _playerInput.enabled = true;

            _characterController = GetComponent<CharacterController>();
            _uiManager = FindFirstObjectByType<UIManager>();
        }

        private void Update()
        {
            if (!IsOwner || _uiManager == null || _uiManager.IsPaused) return;
        
            HandleMovement();
            HandleRotation();
        }

        private void HandleMovement()
        {
            bool isGrounded = IsGrounded();
            if (isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }
            
            Vector3 moveDirection = transform.right * _moveInput.x + transform.forward * _moveInput.y;
            
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

            float currentSpeed = _isSprinting ? sprintSpeed : moveSpeed;
            _characterController.Move(moveDirection * (currentSpeed * Time.deltaTime));

            if (_isJumping && isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                _isJumping = false;
            }

            _velocity.y += gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        private void HandleRotation()
        {
            float mouseX = _lookInput.x * (lookSensitivity / 10f);
            float mouseY = _lookInput.y * (lookSensitivity / 10f);

            _verticalRotation -= mouseY;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -maxLookAngle, maxLookAngle);
            _playerCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position + Vector3.up * 0.03f, Vector3.down, groundCheckDistance);
        }
    
        private void SetShadowsOnly()
        {
            foreach (var rend in playerRenderers)
            {
                rend.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }
        
        public void OnLook(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }
        
        public void OnJump(InputAction.CallbackContext context)
        {
            _isJumping = context.action.triggered && context.phase == InputActionPhase.Performed;
        }
        
        public void OnSprint(InputAction.CallbackContext context)
        {
            _isSprinting = context.action.triggered && context.phase == InputActionPhase.Performed;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + Vector3.up * 0.03f, Vector3.down * groundCheckDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, cameraYOffset, 0), 0.1f);
        }
#endif
    }
}