using _Scripts.Interaction.Interactables;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerGrabController : NetworkBehaviour
    {
        [Header("Throw")]
        [SerializeField] private float throwForceForward = 10f;
        [SerializeField] private float throwForceUp = 2f;
        
        public static PlayerGrabController Instance { get; private set; }
        
        private Transform holdPoint;
        private GrabbableObject _heldObject;
        public bool IsHolding => _heldObject != null;

        private void Update()
        {
            if (!IsOwner || !IsHolding) return;

            Vector3 current = holdPoint.position;
            ServerUpdateHoldPosition(current);
        }

        [ServerRpc]
        private void ServerUpdateHoldPosition(Vector3 position)
        {
            _heldObject?.UpdateHoldPoint(position);
        }
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner) return;

            Instance = this;

            Camera cam = Camera.main;
            
            if (cam != null)
            {
                Transform found = cam.transform.Find("HoldPoint");
                if (found != null)
                    holdPoint = found;
                else
                    Debug.LogWarning("HoldPoint not found under Camera.main!");
            }
        }

        [Client]
        public void TryGrab(GrabbableObject target)
        {
            if(_heldObject != null) return;

            Vector3 clientHoldPos = holdPoint != null ? holdPoint.position : Vector3.zero;
            ServerStartGrab(target, clientHoldPos);
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
            if (!IsHolding) return;
            Vector3 throwForce = holdPoint.forward * throwForceForward + Vector3.up * throwForceUp;
            ServerThrow(throwForce);
        }

        [ServerRpc]
        private void ServerThrow(Vector3 force)
        {
            if (_heldObject == null) return;

            Rigidbody rb = _heldObject.GetRigidbody();
            _heldObject.Detach();
            _heldObject = null;

            RpcConfirmDrop();

            rb.AddForce(force, ForceMode.Impulse);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void ServerStartGrab(GrabbableObject target, Vector3 holdPosition)
        {
            if (_heldObject != null) ServerDrop();

            _heldObject = target;
            target.AttachToPoint(holdPosition, this);

            RpcConfirmGrab(target);
        }

        [ServerRpc]
        public void ServerDrop()
        {
            if (_heldObject != null)
            {
                _heldObject.Detach();
                _heldObject = null;
                RpcConfirmDrop();
            }
        }
        
        [Server]
        public void ForceRelease(GrabbableObject target)
        {
            if (_heldObject == target)
            {
                _heldObject = null;
            }
            
            if (IsOwner)
            {
                _heldObject = null;
            }
        }
        
        [ObserversRpc(BufferLast = true)]
        public void RpcConfirmGrab(GrabbableObject target)
        {
            if (IsOwner)
            {
                _heldObject = target;
            }
        }
        
        [ObserversRpc(BufferLast = true)]
        private void RpcConfirmDrop()
        {
            if (IsOwner)
            {
                _heldObject = null;
            }
        }
    }
}
