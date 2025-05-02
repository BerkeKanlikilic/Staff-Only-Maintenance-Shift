using FishNet.Connection;
using UnityEngine;

namespace _Scripts.Interaction
{
    public interface IInteractable
    {
        string GetInteractionLabel();
        bool CanInteract(NetworkConnection interactor);
        void Interact(NetworkConnection interactor);
    }
}
