using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace _Scripts.UI
{
    public class UIController : NetworkBehaviour
    {
        private UIManager _uiManager;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner) return;

            _uiManager = FindFirstObjectByType<UIManager>();
        }
        
        public void OnPause(InputAction.CallbackContext context)
        {
            _uiManager?.TogglePauseMenu();
        }
    }
}