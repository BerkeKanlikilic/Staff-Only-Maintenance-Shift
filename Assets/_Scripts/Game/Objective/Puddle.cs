using FishNet.Object;
using UnityEngine;

namespace _Scripts.Game.Objective
{
    // Represents a cleanable water puddle for cleaning objectives
    public class Puddle : NetworkBehaviour
    {
        public bool IsCleaned { get; private set; }

        public void Clean()
        {
            if (IsCleaned) return;

            IsCleaned = true;
            Debug.Log($"[Puddle] Cleaned: {name}");
        
            if(IsServerInitialized)
                ObjectiveManager.Instance?.OnObjectiveProgress();
        }
    }
}
