using System;
using System.Collections.Generic;
using _Scripts.UI;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
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

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius = 0.2f;

        private CharacterController _characterController;
        private PlayerInputManager _input;
        private PlayerVisualController _playerVisualController;

        private Vector3 _velocity;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputManager>();
            _playerVisualController = GetComponent<PlayerVisualController>();
        }
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            
            _playerVisualController.HideForLocalPlayer();
        }

        private void Update()
        {
            if (_input == null || !_input.CanProcessInput()) return;

            UpdateMovement();
        }

        private void UpdateMovement()
        {
            bool isGrounded = IsGrounded();
            Vector2 moveInput = _input.MoveInput;
            bool isJumping = _input.ConsumeJump();
            bool isSprinting = _input.SprintHeld;

            if (isGrounded && _velocity.y < 0)
                _velocity.y = -2f;

            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            float speed = isSprinting ? sprintSpeed : moveSpeed;

            if (isGrounded && isJumping)
            {
                _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }

            _velocity.y += gravity * Time.deltaTime;

            Vector3 finalMove = (move * speed) + Vector3.up * _velocity.y;
            
            CollisionFlags flags = _characterController.Move(finalMove * Time.deltaTime);

            if ((flags & CollisionFlags.Above) != 0 && _velocity.y > 0f)
            {
                _velocity.y = 0f;
            }
        }

        private bool IsGrounded()
        {
            Vector3 checkPos = transform.position + Vector3.up * 0.05f; // lift slightly above feet
            return Physics.CheckSphere(checkPos, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector3 checkPos = transform.position + Vector3.up * 0.05f;
            Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
        }
#endif
    }
}