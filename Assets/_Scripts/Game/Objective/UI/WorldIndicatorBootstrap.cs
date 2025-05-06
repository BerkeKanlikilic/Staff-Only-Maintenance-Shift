using UnityEngine;

namespace _Scripts.Game.Objective.UI
{
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