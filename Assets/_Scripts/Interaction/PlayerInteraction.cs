using System.Collections.Generic;
using _Scripts.Interaction.Interactables;
using _Scripts.Player;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Interaction
{
    public class PlayerInteraction : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float maxInteractionDistance = 5f;
        [SerializeField] private float generalDirectionAngle = 0.5f;
        [SerializeField] private LayerMask interactableLayer;

        [Header("Interactable Objects Nearby")]
        [SerializeField] private List<InteractableObject> nearbyObjects = new();
        
        private IInteractable _currentTarget;
        private NetworkObject _targetNetworkObject;
        private Transform _cameraTransform;

        public static PlayerInteraction Instance { get; private set; }
        
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner) return;

            Instance = this;
            _cameraTransform = Camera.main?.transform;
        }
        
        private void Update()
        {
            if (!IsOwner) return;
            UpdateInteractions();
        }
        
        private void UpdateInteractions()
        {
            if (!_cameraTransform || PlayerGrabController.Instance?.IsHolding == true) return;
            
            bool foundTarget = false;

            foreach (var obj in nearbyObjects)
            {
                if (obj == null || !obj.gameObject.activeInHierarchy)
                    continue;
                
                Vector3 directionToObject = obj.GetInteractionPoint() - _cameraTransform.position;
                float dot = Vector3.Dot(_cameraTransform.forward, directionToObject.normalized);

                if (dot > generalDirectionAngle && IsLookingDirectlyAt(obj))
                {
                    if (obj.TryGetComponent<NetworkObject>(out var netObj) &&
                        obj.TryGetComponent<IInteractable>(out var interactable))
                    {
                        if (_currentTarget != interactable)
                        {
                            (_currentTarget as InteractableObject)?.HidePrompt();
                            _currentTarget = interactable;
                            _targetNetworkObject = netObj;
                            (_currentTarget as InteractableObject)?.ShowPrompt();
                        }

                        obj.HideWhiteDot();
                        foundTarget = true;
                    }
                }
                else
                {
                    obj.ShowWhiteDot();
                }
            }

            if (!foundTarget)
                ClearCurrentTarget();
        }
        
        private bool IsLookingDirectlyAt(InteractableObject obj)
        {
            Vector3 rayOrigin = _cameraTransform.position - _cameraTransform.forward * 0.05f;
            Ray ray = new Ray(rayOrigin, _cameraTransform.forward);

            return Physics.Raycast(ray, out var hit, maxInteractionDistance, interactableLayer) &&
                   hit.collider.GetComponentInParent<InteractableObject>() == obj;
        }

        public void OnInteractPerformed()
        {
            if (!IsOwner || PlayerGrabController.Instance?.IsHolding == true) return;

            if (_currentTarget is GrabbableObject grabbable)
            {
                if (PlayerGrabController.Instance != null) PlayerGrabController.Instance.TryGrab(grabbable);
            }
            else if (_targetNetworkObject != null)
            {
                ServerRequestInteract(_targetNetworkObject);
            }

            ClearCurrentTarget();
        }

        public void OnDropPerformed()
        {
            if (!IsOwner) return;
            PlayerGrabController.Instance.TryDrop();
        }

        public void OnThrowPerformed()
        {
            if (!IsOwner) return;
            PlayerGrabController.Instance.TryThrow();
        }

        [ServerRpc]
        private void ServerRequestInteract(NetworkObject target)
        {
            if (!target.TryGetComponent<IInteractable>(out var interactable)) return;

            if (interactable.CanInteract(Owner))
                interactable.Interact(Owner);
        }

        public void AddNearbyObject(InteractableObject obj)
        {
            if (!nearbyObjects.Contains(obj))
                nearbyObjects.Add(obj);
        }

        public void RemoveNearbyObject(InteractableObject obj)
        {
            if (nearbyObjects.Remove(obj) && (InteractableObject)_currentTarget == obj)
                ClearCurrentTarget();
        }

        public void ClearCurrentTarget()
        {
            (_currentTarget as InteractableObject)?.HidePrompt();
            _currentTarget = null;
            _targetNetworkObject = null;
        }
    }
}
