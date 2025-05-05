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
        [SerializeField] private float maxInteractionDistance = 5f;
        [SerializeField] private float generalDirectionAngle = 0.5f;
        [SerializeField] private LayerMask interactableLayer;

        [SerializeField] private List<InteractableObject> nearbyObjects = new();
        private IInteractable _currentTarget;
        private NetworkObject _targetNetworkObject;

        [Header("Input")]
        [SerializeField] private InputActionReference interactAction;
        [SerializeField] private InputActionReference dropAction;
        [SerializeField] private InputActionReference throwAction;

        public static PlayerInteraction Instance;
        private Transform cameraTransform;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner) return;

            Instance = this;
            cameraTransform = Camera.main?.transform;
        }

        private void OnEnable()
        {
            interactAction.action.performed += OnInteractPerformed;
            dropAction.action.performed += OnDropPerformed;
            throwAction.action.performed += OnThrowPerformed;
        }

        private void OnDisable()
        {
            interactAction.action.performed -= OnInteractPerformed;
            dropAction.action.performed -= OnDropPerformed;
            throwAction.action.performed -= OnThrowPerformed;
        }

        private void OnInteractPerformed(InputAction.CallbackContext ctx)
        {
            if (!IsOwner || PlayerGrabController.LocalInstance?.IsHolding == true) return;

            if (_currentTarget is GrabbableObject grabbable)
            {
                PlayerGrabController.LocalInstance.TryGrab(grabbable);
            }
            else if (_targetNetworkObject != null)
            {
                ServerRequestInteract(_targetNetworkObject);
            }

            _currentTarget = null;
            _targetNetworkObject = null;
        }

        private void OnDropPerformed(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            PlayerGrabController.LocalInstance.TryDrop();
        }

        private void OnThrowPerformed(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            PlayerGrabController.LocalInstance.TryThrow();
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

        private void Update()
        {
            if (!IsOwner) return;
            UpdateInteractions();
        }

        private void UpdateInteractions()
        {
            if (!cameraTransform || PlayerGrabController.LocalInstance?.IsHolding == true) return;
            bool foundTarget = false;

            foreach (var obj in nearbyObjects)
            {
                Vector3 directionToObject = obj.GetInteractionPoint() - cameraTransform.position;
                float dot = Vector3.Dot(cameraTransform.forward, directionToObject.normalized);

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
            Vector3 rayOrigin = cameraTransform.position - cameraTransform.forward * 0.05f;
            Ray ray = new Ray(rayOrigin, cameraTransform.forward);

            return Physics.Raycast(ray, out var hit, maxInteractionDistance, interactableLayer) &&
                   hit.collider.GetComponentInParent<InteractableObject>() == obj;
        }

        public void ClearCurrentTarget()
        {
            (_currentTarget as InteractableObject)?.HidePrompt();
            _currentTarget = null;
            _targetNetworkObject = null;
        }
    }
}
