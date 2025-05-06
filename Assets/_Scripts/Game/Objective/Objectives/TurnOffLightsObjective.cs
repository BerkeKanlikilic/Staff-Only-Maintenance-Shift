using System.Collections.Generic;
using _Scripts.Interaction.Interactables;
using UnityEngine;

namespace _Scripts.Game.Objective.Objectives
{
    public class TurnOffLightsObjective : BinaryObjective
    {
        private List<LightSwitch> _lightSwitches;
        
        public TurnOffLightsObjective()
        {
            Id = "lights_off";
            Title = "Lights Out!";
            Description = "Turn off all the lights in the building.";
        }
        
        public override void Initialize()
        {
            base.Initialize();

            _lightSwitches = new List<LightSwitch>(Object.FindObjectsByType<LightSwitch>(FindObjectsSortMode.None));
            Debug.Log($"[TurnOffLightsObjective] Found {_lightSwitches.Count} lights to monitor.");
        }
        
        public override void CheckStatus()
        { 
            if (IsCompleted) return;

            bool allOff = true;

            foreach (var lightSwitch in _lightSwitches)
            {
                if (lightSwitch != null && lightSwitch.IsOn)
                {
                    allOff = false;
                    break;
                }
            }

            if (!allOff) return;
            
            IsCompleted = true;
            Debug.Log("[TurnOffLightsObjective] All lights are OFF. Objective complete!");
            ObjectiveManager.Instance.OnObjectiveProgress();
        }
        
        public override HashSet<GameObject> GetHighlightTargets()
        {
            var targets = new HashSet<GameObject>();

            foreach (var lightSwitch in _lightSwitches)
            {
                if (lightSwitch != null)
                    targets.Add(lightSwitch.gameObject);
            }

            return targets;
        }

    }
}
