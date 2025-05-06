using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Game.Objective.Objectives
{
    public class PickUpItemsObjective : ProgressObjective
    {
        private List<StorableItem> _items;

        public PickUpItemsObjective()
        {
            Id = "pickup_items";
            Title = "Put It Back!";
            Description = "Pick up and return all scattered items to storage.";
        }

        public override void Initialize()
        {
            base.Initialize();

            _items = new List<StorableItem>(Object.FindObjectsByType<StorableItem>(FindObjectsSortMode.None));
            _targetAmount = _items.Count;

            Debug.Log($"[PickUpItemsObjective] Target items: {_targetAmount}");
        }

        public override HashSet<GameObject> GetHighlightTargets()
        {
            var set = new HashSet<GameObject>();
            foreach (var item in _items)
            {
                if (item != null && !item.IsStored)
                    set.Add(item.gameObject);
            }
            return set;
        }

        public override void CheckStatus() { }
    }
}