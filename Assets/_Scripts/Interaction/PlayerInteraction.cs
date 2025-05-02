using System.Collections.Generic;
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

        public static PlayerInteraction Instance;
    
        private Transform cameraTransform;
    
        public override void OnStartClient()
        {
            base.OnStartClient();
        
            if(!IsOwner) return;
        
            Instance = this;
        
            cameraTransform = Camera.main?.transform;
        }
    
        private void OnEnable()
        {
            if (interactAction != null)
                interactAction.action.performed += OnInteractPerformed;
        }

        private void OnDisable()
        {
            if (interactAction != null)
                interactAction.action.performed -= OnInteractPerformed;
        }
    
        private void OnInteractPerformed(InputAction.CallbackContext ctx)
        {
            if (!IsOwner || _currentTarget == null || _targetNetworkObject == null) return;
            
            ServerRequestInteract(_targetNetworkObject);
            _currentTarget = null;
            _targetNetworkObject = null;
        }

        [ServerRpc]
        private void ServerRequestInteract(NetworkObject target)
        {
            if (!target.TryGetComponent<IInteractable>(out var interactable)) return;
            
            if(interactable.CanInteract(Owner))
                interactable.Interact(Owner);
        }

        public void AddNearbyObject(InteractableObject obj)
        {
            if (!nearbyObjects.Contains(obj))
            {
                nearbyObjects.Add(obj);
            }
        }

        public void RemoveNearbyObject(InteractableObject obj)
        {
            if (nearbyObjects.Contains(obj))
            {
                nearbyObjects.Remove(obj);
            }

            if (_currentTarget == obj)
            {
                ClearCurrentTarget();
            }
        }

        private void Update()
        {
            if (!IsOwner) return;
        
            UpdateInteractions();
        }

        void UpdateInteractions()
        {
            if (!cameraTransform) return;

            bool foundTarget = false;
        
            foreach (var obj in nearbyObjects)
            {
                Vector3 directionToObject = obj.transform.position - cameraTransform.position;
                float dot = Vector3.Dot(cameraTransform.forward, directionToObject.normalized);

                if (dot > generalDirectionAngle)
                {
                    bool isLookingDirectly = IsLookingDirectlyAt(obj);

                    if (isLookingDirectly)
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
                else
                {
                    obj.HideWhiteDot();
                }
            }

            if (!foundTarget)
            {
                ClearCurrentTarget();
            }
        }

        private bool IsLookingDirectlyAt(InteractableObject obj)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxInteractionDistance, interactableLayer))
            {
                return hit.collider.gameObject == obj.gameObject;
            }

            return false;
        }

        private void ClearCurrentTarget()
        {
            (_currentTarget as InteractableObject)?.HidePrompt();
            _currentTarget = null;
            _targetNetworkObject = null;
        }
    }
}
