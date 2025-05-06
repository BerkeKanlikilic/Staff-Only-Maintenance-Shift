using FishNet.Object;
using UnityEngine;

namespace _Scripts.Game.Objective
{
    // Trigger zone that marks StorableItems as stored when they enter
    [RequireComponent(typeof(Collider))]
    public class StorageZone : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!IsServerInitialized) return;

            if (other.TryGetComponent(out StorableItem item) && !item.IsStored)
            {
                item.MarkStored();
            }
        }
    }
}