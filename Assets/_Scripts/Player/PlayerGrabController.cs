using _Scripts.Interaction;
using _Scripts.Interaction.Interactables;
using _Scripts.UI;
using FishNet.Object;
using FishNet.Connection;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerGrabController : NetworkBehaviour
    {
        [Header("Throw Settings")]
        [SerializeField] private float throwForceForward = 10f;
        [SerializeField] private float throwForceUp = 2f;
        
        public bool IsHolding => _heldObject != null;

        public static PlayerGrabController LocalInstance { get; private set; }
        
        private Transform holdPoint;
        private GrabbableObject _heldObject;
        private GrabbableObject _pendingGrab;
        private float inputBufferTimer = 0f;
        private const float inputBufferDuration = 0.3f;
        private GrabbableObject _lastHeldObject;
        
        private void Update()
        {
            if (!IsOwner) return;

            if (_pendingGrab != null)
            {
                inputBufferTimer += Time.deltaTime;
                if (inputBufferTimer <= inputBufferDuration)
                {
                    TryGrab(_pendingGrab);
                }
                else
                {
                    _pendingGrab = null;
                    inputBufferTimer = 0f;
                }
            }

            if (IsHolding && holdPoint != null)
            {
                ServerUpdateHoldPosition(holdPoint.position);
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner) return;

            LocalInstance = this;
            Camera cam = Camera.main;
            if (cam != null)
            {
                holdPoint = cam.transform.Find("HoldPoint");
                if (holdPoint == null)
                    Debug.LogWarning("HoldPoint not found under Camera.main!");
            }
        }

        [Client]
        public void TryGrab(GrabbableObject target)
        {
            if (_heldObject != null)
            {
                if (_heldObject == target)
                {
                    Debug.Log("Already holding this object.");
                    return;
                }
                
                Debug.LogWarning("Client thinks it's holding something. Trying to recover...");
                TryDrop();
                _pendingGrab = target;
                inputBufferTimer = 0f;
                return;
            }

            if (holdPoint == null)
            {
                _pendingGrab = target;
                inputBufferTimer = 0f;
                return;
            }

            _pendingGrab = null;
            inputBufferTimer = 0f;

            target.ShowGrabPreview();

            ServerStartGrab(target, holdPoint.position);
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
            if (!IsHolding || holdPoint == null) return;
            Vector3 force = holdPoint.forward * throwForceForward + Vector3.up * throwForceUp;
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
            if (_heldObject != null) ServerDrop();
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
            else
                Debug.LogWarning(">>> TargetConfirmDetach: _heldObject was already null!");;
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
    }
}
