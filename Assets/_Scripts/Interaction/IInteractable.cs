using FishNet.Connection;
using UnityEngine;

namespace _Scripts.Interaction
{
    // Interface for all objects that can be interacted with
    public interface IInteractable
    {
        // Returns whether the given player is allowed to interact
        bool CanInteract(NetworkConnection interactor);
        
        // Defines what happens when the player interacts
        void Interact(NetworkConnection interactor);
    }
}
