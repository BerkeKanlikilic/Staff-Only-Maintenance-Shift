using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace _Scripts.UI
{
    public class UIController : NetworkBehaviour
    {
        [Header("Input Actions")]
        [SerializeField] private InputActionReference pauseAction;

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if(!IsOwner) enabled = false;
        }

        private void OnEnable()
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPause;
        }

        private void OnDisable()
        {
            pauseAction.action.performed -= OnPause;
            pauseAction.action.Disable();
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            Debug.Log($"Pressed Pause");
            UIManager.Instance?.TogglePauseMenu();
        }
    }
}