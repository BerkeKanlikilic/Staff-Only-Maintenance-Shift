using _Scripts.Player;
using FishNet.Connection;
using UnityEngine;

namespace _Scripts.Interaction.Interactables
{
    public class GrabbableObject : InteractableObject
    {
        public virtual void OnUse() { }

        public virtual void ShowGrabPreview() { }
    
        public virtual bool TryGrab(PlayerGrabController grabber, Vector3 holdPosition) { return false; }

        public virtual void UpdateHoldPoint(Vector3 worldPosition) { }

        public virtual void Drop() { }

        public virtual void Throw(Vector3 force) { }

        public virtual void OnGrabConfirmedClient() { }

        public virtual void OnDetachConfirmedClient() { }

        public override void Interact(NetworkConnection interactor) { }
    }
}
