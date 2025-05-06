using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Game.Objective
{
    public abstract class BinaryObjective : IObjective
    {
        public string Id { get; protected set; }
        public string Title { get; protected set; }
        public string Description { get; protected set; }
        public bool IsCompleted { get; protected set; }

        public virtual void Initialize()
        {
            IsCompleted = false;
        }

        public abstract void CheckStatus(); // e.g., are all lights off?

        public virtual void OnProgress()
        {
            // Not needed for binary.
        }

        public virtual HashSet<GameObject> GetHighlightTargets() => new();
    }
}
