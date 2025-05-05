using System;
using _Scripts.Interaction;
using _Scripts.UI;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Player
{
    public class PlayerInputManager : NetworkBehaviour
    {
        [Header("Input Actions")]
        [SerializeField] private InputActionAsset inputActions;

        [Header("Gameplay Actions")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference lookAction;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputActionReference sprintAction;
        [SerializeField] private InputActionReference interactAction;
        [SerializeField] private InputActionReference dropAction;
        [SerializeField] private InputActionReference throwAction;
        
        private InputAction _pauseAction;
        
        // Input values
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool SprintHeld { get; private set; }
        public bool JumpHeld => jumpAction.action.IsPressed();

        // Internal
        private bool _jumpPressed;
        private bool _gameplayEnabled = true;
        private UIController _uiController;
        private PlayerInteraction _interaction;
        private PlayerGrabController _grab;

        private const string GAMEPLAY_MAP = "Gameplay";
        private const string UI_MAP = "UI";

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner) return;
            
            _uiController = GetComponent<UIController>();
            _interaction = GetComponent<PlayerInteraction>();
            _grab = GetComponent<PlayerGrabController>();
            
            RebindPauseAction(inputActions.FindActionMap(GAMEPLAY_MAP));
        }

        private void OnEnable()
        {
            SwitchToGameplayMap();
            RegisterCallbacks();
        }

        private void OnDisable()
        {
            UnregisterCallbacks();
        }

        private void Update()
        {
            if (!CanProcessInput()) return;
            
            MoveInput = moveAction.action.ReadValue<Vector2>();
            LookInput = lookAction.action.ReadValue<Vector2>();
            SprintHeld = sprintAction.action.IsPressed();
            
#if UNITY_EDITOR
            if (_gameplayEnabled && !IsActionMapActive(GAMEPLAY_MAP))
            {
                Debug.LogWarning("Gameplay map was expected but not active — resyncing.");
                SwitchToGameplayMap();
            }
#endif
        }

        public bool ConsumeJump()
        {
            if (_jumpPressed)
            {
                _jumpPressed = false;
                return true;
            }

            return false;
        }

        public bool CanProcessInput()
        {
            return _gameplayEnabled && IsActionMapActive(GAMEPLAY_MAP);
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            _jumpPressed = true;
        }
        
        private void OnPause(InputAction.CallbackContext ctx)
        {
            if (!_uiController) return;

            bool willPause = !_uiController.IsPaused;
            _uiController.OnPause();

            if (willPause)
                SwitchToUIMap();
            else
                SwitchToGameplayMap();
        }
        
        private void OnInteract(InputAction.CallbackContext ctx)
        {
            _interaction?.OnInteractPerformed();
        }
        
        private void OnDrop(InputAction.CallbackContext ctx)
        {
            _interaction?.OnDropPerformed();
        }
        
        private void OnThrow(InputAction.CallbackContext ctx)
        {
            _interaction?.OnThrowPerformed();
        }
        
        private void SwitchToGameplayMap()
        {
            if (inputActions == null) return;
            
            var gameplayMap = inputActions.FindActionMap(GAMEPLAY_MAP, true);
            var uiMap = inputActions.FindActionMap(UI_MAP, true);

            uiMap.Disable();
            gameplayMap.Enable();
            _gameplayEnabled = true;

            RebindPauseAction(gameplayMap);
        }
        
        private void SwitchToUIMap()
        {
            if (inputActions == null) return;
            
            var gameplayMap = inputActions.FindActionMap(GAMEPLAY_MAP, true);
            var uiMap = inputActions.FindActionMap(UI_MAP, true);

            gameplayMap.Disable();
            uiMap.Enable();
            _gameplayEnabled = false;

            RebindPauseAction(uiMap);
        }

        private void RebindPauseAction(InputActionMap map)
        {
            if (_pauseAction != null)
                _pauseAction.performed -= OnPause;

            _pauseAction = map.FindAction("Pause");
            
            if(_pauseAction != null)
                _pauseAction.performed += OnPause;
            else
                Debug.LogWarning($"Pause action not found in map '{map.name}'!");
        }
        
        private bool IsActionMapActive(string mapName)
        {
            var map = inputActions.FindActionMap(mapName, true);
            return map?.enabled ?? false;
        }

        private void RegisterCallbacks()
        {
            jumpAction.action.performed += OnJump;
            interactAction.action.performed += OnInteract;
            dropAction.action.performed += OnDrop;
            throwAction.action.performed += OnThrow;
        }

        private void UnregisterCallbacks()
        {
            jumpAction.action.performed -= OnJump;
            interactAction.action.performed -= OnInteract;
            dropAction.action.performed -= OnDrop;
            throwAction.action.performed -= OnThrow;
        }
    }
}
