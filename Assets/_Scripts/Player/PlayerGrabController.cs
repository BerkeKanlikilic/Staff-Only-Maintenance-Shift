using _Scripts.Interaction;
using _Scripts.Interaction.Interactables;
using _Scripts.UI;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine;

namespace _Scripts.Player
{
    // Manages object grabbing, holding, dropping, and throwing
    public class PlayerGrabController : NetworkBehaviour
    {
        [Header("Throw Settings")]
        [SerializeField] private float throwForceForward = 10f;
        [SerializeField] private float throwForceUp = 2f;
        
        public static PlayerGrabController Instance { get; private set; }
        public bool IsHolding => _heldObject != null;
        public Transform HoldPoint => _holdPoint;
        
        private Transform _holdPoint;
        private GrabbableObject _heldObject;
        private GrabbableObject _pendingGrab;
        private GrabbableObject _lastHeldObject;
        private float _inputBufferTimer = 0f;
        private const float InputBufferDuration = 0.3f;
        
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner) return;

            Instance = this;
            
            // Locate hold point under the camera
            Camera cam = Camera.main;
            if (cam != null)
                _holdPoint = cam.transform.Find("HoldPoint");
        }
        
        private void Update()
        {
            if (!IsOwner) return;

            // Retry grab within buffer window
            if (_pendingGrab)
            {
                _inputBufferTimer += Time.deltaTime;
                if (_inputBufferTimer <= InputBufferDuration)
                {
                    TryGrab(_pendingGrab);
                }
                else
                {
                    _pendingGrab = null;
                    _inputBufferTimer = 0f;
                }
            }

            // Keep updating held object’s position
            if (IsHolding && _holdPoint != null)
            {
                ServerUpdateHoldPosition(_holdPoint.position);
            }
        }

        [Client]
        public void TryGrab(GrabbableObject target)
        {
            if (_heldObject != null)
            {
                if (_heldObject == target) return;
                
                // Already holding something — attempt recovery
                TryDrop();
                _pendingGrab = target;
                _inputBufferTimer = 0f;
                return;
            }

            if (_holdPoint == null)
            {
                _pendingGrab = target;
                _inputBufferTimer = 0f;
                return;
            }

            _pendingGrab = null;
            _inputBufferTimer = 0f;

            target.ShowGrabPreview();
            ServerStartGrab(target, _holdPoint.position);
        }

        [Client]
        public void TryDrop()
        {
            if (!IsHolding) return;
            ServerDrop();
        }

        [Client]
        public void TryThrow()
        {
            if (!IsHolding || _holdPoint == null) return;
            Vector3 force = _holdPoint.forward * throwForceForward + Vector3.up * throwForceUp;
            ServerThrow(force);
        }

        [ServerRpc]
        private void ServerUpdateHoldPosition(Vector3 position)
        {
            _heldObject?.UpdateHoldPoint(position);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ServerStartGrab(GrabbableObject target, Vector3 holdPosition)
        {
            if (_heldObject)
                ServerDrop();
            
            if (target.TryGrab(this, holdPosition))
            {
                _heldObject = target;
                TargetConfirmGrab(Owner, target);
            }
        }

        [ServerRpc]
        public void ServerDrop()
        {
            if (_heldObject == null) return;
            _heldObject.Drop();
            TargetConfirmDetach(Owner);
            _heldObject = null;
        }

        [ServerRpc]
        private void ServerThrow(Vector3 force)
        {
            if (_heldObject == null) return;
            _heldObject.Throw(force);
            TargetConfirmDetach(Owner);
            _heldObject = null;
        }

        [TargetRpc]
        private void TargetConfirmGrab(NetworkConnection conn, GrabbableObject target)
        {
            _heldObject = target;
            _lastHeldObject = target;
            _heldObject.OnGrabConfirmedClient();
            
            PlayerInteraction.Instance?.ClearCurrentTarget();
        }

        [TargetRpc]
        private void TargetConfirmDetach(NetworkConnection conn)
        {
            var obj = _heldObject != null ? _heldObject : _lastHeldObject;
            _heldObject = null;
            _lastHeldObject = null;
            
            if (obj)
                obj.OnDetachConfirmedClient();
        }

        [Server]
        public void ForceRelease(GrabbableObject target, bool notifyClient = false)
        {
            if (_heldObject == target)
                _heldObject = null;

            if (notifyClient)
            {
                TargetConfirmDetach(Owner);
            }
        }

        public void UseHeldObject()
        {
            _heldObject?.OnUse();
        }
        
        public bool IsHoldingObject(GrabbableObject obj)
        {
            return _heldObject == obj;
        }
    }
}
