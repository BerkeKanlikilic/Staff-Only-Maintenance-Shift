using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Interaction.Interactables
{
    public class LightSwitch : InteractableObject
    {
        [Header("Settings")]
        [SerializeField] private bool isInitiallyOn = false;
        [SerializeField] private List<Light> controlledLights;
        [SerializeField] private MeshRenderer lightOnMeshRenderer;
        [SerializeField] private MeshRenderer lightOffMeshRenderer;

        private bool _isOn = false;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            _isOn = isInitiallyOn;

            UpdateLightVisual(_isOn);
            if (IsServerStarted)
                RpcUpdateLight(_isOn);
        }

        public override void Interact(NetworkConnection interactor)
        {
            if (!IsServerStarted) return;

            _isOn = !_isOn;
            RpcUpdateLight(_isOn);
        }

        [ObserversRpc]
        private void RpcUpdateLight(bool state)
        {
            UpdateLightVisual(state);
        }
        
        private void UpdateLightVisual(bool state)
        {
            lightOnMeshRenderer.enabled = state;
            lightOffMeshRenderer.enabled = !state;
            foreach (var controlledLight in controlledLights)
                controlledLight.enabled = state;
        }
    }
}