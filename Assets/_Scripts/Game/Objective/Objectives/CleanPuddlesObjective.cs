using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Game.Objective.Objectives
{
    // Progress-based objective: clean all puddles in the scene
    public class CleanPuddlesObjective : ProgressObjective
    {
        private List<Puddle> _puddles;

        public CleanPuddlesObjective()
        {
            Id = "clean_puddles";
            Title = "Clean It Up!";
            Description = "Mop all the puddles in the restaurant.";
        }

        public override void Initialize()
        {
            base.Initialize();

            _puddles = new List<Puddle>(Object.FindObjectsByType<Puddle>(FindObjectsSortMode.None));
            _targetAmount = _puddles.Count;

            Debug.Log($"[CleanPuddlesObjective] Target puddles to clean: {_targetAmount}");
        }

        public override void CheckStatus()
        {
            // No periodic check needed for this one
        }

        public override HashSet<GameObject> GetHighlightTargets()
        {
            var set = new HashSet<GameObject>();
            foreach (var puddle in _puddles)
            {
                if (puddle != null && !puddle.IsCleaned)
                    set.Add(puddle.gameObject);
            }

            return set;
        }
    }
}
