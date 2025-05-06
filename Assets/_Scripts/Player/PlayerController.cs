using System;
using System.Collections.Generic;
using _Scripts.Game;
using _Scripts.UI;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

namespace _Scripts.Player
{
    // This component controls player movement (walking, sprinting, jumping)
    // in a networked multiplayer setup using FishNet.
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : NetworkBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 7.5f;
        [SerializeField] private float sprintSpeed = 11.5f;
        
        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 1f;
        [SerializeField] private float gravity = -9.81f;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius = 0.2f;

        private CharacterController _characterController;
        private PlayerInputManager _input;
        private PlayerVisualController _playerVisualController;

        private Vector3 _velocity;

        private void Awake()
        {
            // Cache required component references
            _characterController = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputManager>();
            _playerVisualController = GetComponent<PlayerVisualController>();
        }
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            // Disable this component on clients that don't own this player
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            
            // Hide visuals (like nameplate) for the local player
            _playerVisualController.HideForLocalPlayer();
        }

        private void Update()
        {
            // Skip movement updates if the game is frozen or input isn't allowed
            if (GameManager.GameState.IsGameFrozen) return;
            if (_input == null || !_input.CanProcessInput()) return;

            UpdateMovement();
        }

        // Handles walking, sprinting, jumping, and gravity
        private void UpdateMovement()
        {
            bool isGrounded = IsGrounded();
            Vector2 moveInput = _input.MoveInput;
            bool isJumping = _input.ConsumeJump();
            bool isSprinting = _input.SprintHeld;
            
            // Reset downward velocity when grounded
            if (isGrounded && _velocity.y < 0)
                _velocity.y = -2f;

            // Get directional movement vector based on input
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            float speed = isSprinting ? sprintSpeed : moveSpeed;
            
            // Apply jump force if grounded and jump input is triggered
            if (isGrounded && isJumping)
            {
                _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }

            // Apply gravity to vertical velocity
            _velocity.y += gravity * Time.deltaTime;

            // Final movement vector includes horizontal + vertical movement
            Vector3 finalMove = (move * speed) + Vector3.up * _velocity.y;
            
            // Move the character controller and detect ceiling collisions
            CollisionFlags flags = _characterController.Move(finalMove * Time.deltaTime);

            // Stop upward movement if hitting ceiling
            if ((flags & CollisionFlags.Above) != 0 && _velocity.y > 0f)
            {
                _velocity.y = 0f;
            }
        }

        // Performs a sphere check just above the player's feet to see if grounded
        private bool IsGrounded()
        {
            Vector3 checkPos = transform.position + Vector3.up * 0.05f; // lift slightly above feet
            return Physics.CheckSphere(checkPos, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
        }

#if UNITY_EDITOR
        // Draws ground check gizmo in editor for debugging
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector3 checkPos = transform.position + Vector3.up * 0.05f;
            Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
        }
#endif
    }
}