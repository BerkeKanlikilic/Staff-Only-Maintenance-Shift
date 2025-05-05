using System.Collections.Generic;
using _Scripts.UI;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace _Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 7.5f;
        [SerializeField] private float sprintSpeed = 11.5f;
        
        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 1f;
        [SerializeField] private float gravity = -9.81f;

        [Header("Look Settings")]
        [Range(1,10)] [SerializeField] private float lookSensitivity = 2f;
        [SerializeField] private float maxLookAngle = 80f;

        [Header("Camera Settings")]
        [SerializeField] private float cameraYOffset = 1.4f;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckYOffset = 0.2f;
        [SerializeField] private float groundCheckRadius = 0.1f;

        [Header("References")]
        [SerializeField] private List<Renderer> playerRenderers;
        [SerializeField] private TMP_Text nameText;
        
        [Header("Input Actions")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference lookAction;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputActionReference sprintAction;
    
        private Camera _playerCamera;
        private CharacterController _characterController;
        private UIManager _uiManager;
        private Vector3 _velocity;
        private float _verticalRotation;

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

            _characterController = GetComponent<CharacterController>();
            _uiManager = FindFirstObjectByType<UIManager>();
        }

        private void Update()
        {
            if (!IsOwner || !_uiManager) return;
            
            if(!_uiManager.IsPaused)
            {
                _moveInput = moveAction.action.ReadValue<Vector2>();
                _lookInput = lookAction.action.ReadValue<Vector2>();
                _isJumping = jumpAction.action.triggered;
                _isSprinting = sprintAction.action.IsPressed();


                HandleMovement();
                HandleRotation();
            }
            HandleGravity();
        }

        private void HandleMovement()
        {
            bool isGrounded = IsGrounded();

            Vector3 moveDirection = transform.right * _moveInput.x + transform.forward * _moveInput.y;
            float currentSpeed = _isSprinting ? sprintSpeed : moveSpeed;
            _characterController.Move(moveDirection.normalized * (currentSpeed * Time.deltaTime));

            if (isGrounded && _velocity.y < 0f)
            {
                _velocity.y = -2f;
            }

            if (isGrounded && _isJumping)
            {
                _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                _isJumping = false;
            }
        }

        private void HandleGravity()
        {
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
            Vector3 checkPos = transform.position + Vector3.down * groundCheckYOffset;
            return Physics.CheckSphere(checkPos, groundCheckRadius, groundLayer);
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
            if (context.performed)
                _isJumping = true;
        }
        
        public void OnSprint(InputAction.CallbackContext context)
        {
            _isSprinting = context.action.triggered && context.phase == InputActionPhase.Performed;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blueViolet;
            Vector3 checkPos = transform.position + Vector3.down * groundCheckYOffset;
            Gizmos.DrawWireSphere(checkPos, groundCheckRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, cameraYOffset, 0), 0.1f);
        }
#endif
    }
}