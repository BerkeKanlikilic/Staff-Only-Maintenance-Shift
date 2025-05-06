using UnityEngine;

namespace _Scripts.Game.Objective.UI
{
    // Ensures all indicators in the scene are activated on clients at runtime
    public class WorldIndicatorBootstrap : MonoBehaviour
    {
        private void Start()
        {
            // This runs on ALL clients regardless of ownership
            var indicators = FindObjectsByType<WorldIndicator>(FindObjectsSortMode.None);

            foreach (var indicator in indicators)
            {
                // Force awake logic if Start didn't run (if Unity skipped due to prefab state)
                indicator.enabled = true;
            }
        }
    }
}