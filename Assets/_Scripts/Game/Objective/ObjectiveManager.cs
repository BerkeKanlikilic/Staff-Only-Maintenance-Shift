using System.Collections.Generic;
using _Scripts.Game.Objective.Objectives;
using _Scripts.Game.Objective.UI;
using _Scripts.UI;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Game.Objective
{
    public class ObjectiveManager : NetworkBehaviour
    {
        public static ObjectiveManager Instance { get; private set; }

        private readonly List<IObjective> _objectives = new();
        private int _currentObjectiveIndex = -1;
        
        public IObjective Current => (_currentObjectiveIndex >= 0 && _currentObjectiveIndex < _objectives.Count)
            ? _objectives[_currentObjectiveIndex]
            : null;
        
        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }

        [Server]
        public void InitializeObjectives()
        {
            _objectives.Clear();

            _objectives.Add(new PickUpItemsObjective());
            _objectives.Add(new CleanPuddlesObjective());
            _objectives.Add(new TurnOffLightsObjective());
            _objectives.Add(new ExitAndLockObjective());
            
            _currentObjectiveIndex = 0;
            _objectives[_currentObjectiveIndex].Initialize();
            
            SyncIndicators();
            
            RpcUpdateObjectiveUI(Current.Title, Current.Description);
        }
        
        [Server]
        public void OnObjectiveProgress()
        {
            if (Current == null)
                return;

            Current.OnProgress();

            if (Current.IsCompleted)
            {
                Debug.Log($"[ObjectiveManager] Objective completed: {Current.Title}");

                AdvanceToNextObjective();
            }
        }
        
        [Server]
        private void AdvanceToNextObjective()
        {
            _currentObjectiveIndex++;

            if (_currentObjectiveIndex < _objectives.Count)
            {
                Debug.Log($"[ObjectiveManager] Advancing to: {_objectives[_currentObjectiveIndex].Title}");
                _objectives[_currentObjectiveIndex].Initialize();

                SyncIndicators();
                RpcUpdateObjectiveUI(Current.Title, Current.Description);
            }
            else
            {
                Debug.Log("[ObjectiveManager] All objectives complete. Game over?");
                RpcHideObjectiveUI();
                RpcClearAllIndicators();
                RpcGameCompleteFadeout();
            }
        }

        [ObserversRpc]
        private void RpcGameCompleteFadeout()
        {
            UIManager.Instance?.ShowGameEndScreen("All objectives complete!");
        }

        [ObserversRpc]
        private void RpcUpdateObjectiveUI(string title, string description)
        {
            UIManager.Instance?.ObjectiveUI?.ShowObjective(title, description);
        }
        
        [ObserversRpc]
        private void RpcHideObjectiveUI()
        {
            UIManager.Instance?.ObjectiveUI?.HideObjective();
        }
        
        private void Update()
        {
            if (IsServerInitialized && Current != null)
            {
                Current.CheckStatus();
            }
        }
        
        [Server]
        private void SyncIndicators()
        {
            if (Current == null)
            {
                Debug.LogWarning("[ObjectiveManager] Tried to sync indicators with no current objective.");
                return;
            }

            var targets = Current.GetHighlightTargets();
            List<NetworkObject> netTargets = new();

            foreach (var go in targets)
            {
                if (go.TryGetComponent<NetworkObject>(out var netObj))
                    netTargets.Add(netObj);
            }

            RpcSetWorldIndicators(netTargets.ToArray());
        }
        
        [ObserversRpc]
        private void RpcSetWorldIndicators(NetworkObject[] targets)
        {
            WorldIndicatorRegistry.SetOnlyVisible(targets);
        }
        
        [ObserversRpc]
        private void RpcClearAllIndicators()
        {
            WorldIndicatorRegistry.SetAllVisible(false);
        }
    }
}
