using FishNet.Object;
using UnityEngine;

namespace _Scripts.Game.Objective
{
    // Marks an object that counts toward a storage-related objective
    public class StorableItem : NetworkBehaviour
    {
        public bool IsStored { get; private set; }

        public void MarkStored()
        {
            if (IsStored)
                return;

            IsStored = true;

            if (IsServerInitialized)
            {
                Debug.Log($"[StorableItem] {name} stored.");
                _Scripts.Game.Objective.ObjectiveManager.Instance?.OnObjectiveProgress();
            }
        }
    }
}
