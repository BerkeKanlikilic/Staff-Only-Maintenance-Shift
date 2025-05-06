using System;
using System.Collections;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace _Scripts.UI
{
    // Handles input-based toggling of the pause menu
    public class UIController : NetworkBehaviour
    {
        public bool IsPaused { get; private set; }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner)
            {
                enabled = false;
                return;
            }
        }

        public void OnPause()
        {
            IsPaused = !IsPaused;
            UIManager.Instance.TogglePauseMenu();
        }
    }
}