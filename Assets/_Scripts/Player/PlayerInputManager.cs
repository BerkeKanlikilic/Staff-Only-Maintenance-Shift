using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Player
{
    public class PlayerInputManager : NetworkBehaviour
    {
        [Header("Input Actions")]
        public InputActionReference moveAction;
        public InputActionReference lookAction;
        public InputActionReference jumpAction;
        public InputActionReference sprintAction;
        public InputActionReference interactAction;
        public InputActionReference dropAction;
        public InputActionReference throwAction;
        public InputActionReference pauseAction;
        
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool SprintHeld { get; private set; }

        private void OnEnable()
        {
            moveAction.action.Enable();
            lookAction.action.Enable();
            jumpAction.action.Enable();
            sprintAction.action.Enable();
            interactAction.action.Enable();
            dropAction.action.Enable();
            throwAction.action.Enable();
            pauseAction.action.Enable();
        }
        private void OnDisable()
        {
            moveAction.action.Disable();
            lookAction.action.Disable();
            jumpAction.action.Disable();
            sprintAction.action.Disable();
            interactAction.action.Disable();
            dropAction.action.Disable();
            throwAction.action.Disable();
            pauseAction.action.Disable();
        }

        private void Update()
        {
            MoveInput = moveAction.action.ReadValue<Vector2>();
            LookInput = lookAction.action.ReadValue<Vector2>();
            JumpPressed = jumpAction.action.triggered;
            SprintHeld = sprintAction.action.IsPressed();
        }

        public void ClearJumpFlag() => JumpPressed = false;
    }
}
