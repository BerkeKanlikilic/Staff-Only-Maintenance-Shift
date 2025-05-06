using FishNet.Object;
using UnityEngine;

namespace _Scripts.Game.Objective
{
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