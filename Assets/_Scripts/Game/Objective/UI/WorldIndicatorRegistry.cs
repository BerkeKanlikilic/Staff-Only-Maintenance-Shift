using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Game.Objective.UI
{
    // Central list of all world indicators — enables toggling visibility globally or by targets
    public static class WorldIndicatorRegistry
    {
        private static readonly List<WorldIndicator> indicators = new();

        public static void Register(WorldIndicator wi)
        {
            if (!indicators.Contains(wi))
                indicators.Add(wi);
        }

        public static void Unregister(WorldIndicator wi)
        {
            indicators.Remove(wi);
        }

        public static void SetAllVisible(bool visible)
        {
            foreach (var indicator in indicators)
            {
                if (indicator != null)
                    indicator.SetVisible(visible);
            }
        }

        public static void SetOnlyVisible(NetworkObject[] targets)
        {
            var targetSet = new HashSet<NetworkObject>(targets);

            foreach (var indicator in indicators)
            {
                if (indicator == null) continue;
                if (!indicator.TryGetComponent<NetworkObject>(out var netObj)) continue;

                bool shouldShow = targetSet.Contains(netObj);
                indicator.SetVisible(shouldShow);
            }
        }
    }
}
