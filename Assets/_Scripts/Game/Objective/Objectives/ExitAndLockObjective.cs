using System.Collections.Generic;
using UnityEngine;
using _Scripts.Interaction.Interactables;

namespace _Scripts.Game.Objective.Objectives
{
    // Binary objective: wait until the exit door is closed after being opened
    public class ExitAndLockObjective : BinaryObjective
    {
        private Door _door;
        private bool _wasOpenBefore = false;

        public ExitAndLockObjective()
        {
            Id = "exit_and_lock";
            Title = "Lock It Down!";
            Description = "Exit the store and lock the main door behind you.";
        }

        public override void Initialize()
        {
            base.Initialize();

            var doorObject = GameObject.FindWithTag("ExitDoor");
            if (doorObject != null)
                _door = doorObject.GetComponent<Door>();
            
            if (_door == null)
                Debug.LogError("[ExitAndLockObjective] No Door found in scene!");
        }

        public override void CheckStatus()
        {
            if (_door == null) return;

            // Watch for transition: open → closed
            if (_wasOpenBefore && !_door.GetIsOpen())
            {
                Debug.Log("[ExitAndLockObjective] Door closed after being opened. Objective complete.");
                OnProgress(); // Marks complete
            }

            _wasOpenBefore |= _door.GetIsOpen(); // Remember if it was ever opened
        }

        public override HashSet<GameObject> GetHighlightTargets()
        {
            if (_door != null)
                return new HashSet<GameObject> { _door.gameObject };
            return new HashSet<GameObject>();
        }
        
        public override void OnProgress()
        {
            if (IsCompleted) return;

            IsCompleted = true;
            Debug.Log("[ExitAndLockObjective] Marked as completed.");
            ObjectiveManager.Instance?.OnObjectiveProgress();
        }
    }
}