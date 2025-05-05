using _Scripts.Game;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Interaction.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class StaffRoomExitDoor : Door
    {
        private bool _doorOpened;

        public override void Interact(NetworkConnection interactor)
        {
            base.Interact(interactor);
            
            TryOpenDoorServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void TryOpenDoorServerRpc()
        {
            if (_doorOpened) return;

            _doorOpened = true;
            
            // Optional: play animation, unlock door, etc.
            Debug.Log("Exit door opened. Starting the game...");
            
            GameManager.Instance.OnExitDoorOpened();
        }
    }
}
