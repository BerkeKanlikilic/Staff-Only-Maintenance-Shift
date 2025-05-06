using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Interaction.Interactables
{
    public class MopProp : InteractableObject
    {
        public override bool CanInteract(NetworkConnection conn) => true;

        public override void Interact(NetworkConnection conn)
        {
            if (!base.IsServerStarted) return;

            var player = conn.FirstObject;
            if (player != null && player.TryGetComponent(out PlayerToolManager toolManager))
            {
                toolManager.SetMopActive(true);
            }

            Despawn();
        }
    }
}