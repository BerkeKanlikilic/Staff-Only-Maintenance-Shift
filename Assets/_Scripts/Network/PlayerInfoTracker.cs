using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Game;
using _Scripts.Game.GameState;
using _Scripts.UI;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;

namespace _Scripts.Network
{
    // Tracks player names and connection states for the session.
    // Syncs data to clients and updates the UI accordingly.
    public class PlayerInfoTracker : NetworkBehaviour
    {
        private UIManager _uiManager;

        private int _playerCount = 0;
        
        public static event Action<int, string> OnNameChange;
        
        private readonly SyncDictionary<int, string> _playerNames = new();
        
        private static PlayerInfoTracker _instance;
        
        private void Awake()
        {
            _instance = this;
            _uiManager = FindFirstObjectByType<UIManager>();
            
            // Trigger name update events when dictionary changes
            _playerNames.OnChange += (op, key, value, asServer) =>
            {
                OnNameChange?.Invoke(key, value);
            };
            _playerNames.OnChange += OnPlayerNameChanged;
        }

        public void OnPlayerNameChanged(SyncDictionaryOperation op, int clientId, string name, bool asServer)
        {
            if(op is SyncDictionaryOperation.Add or SyncDictionaryOperation.Set)
                _uiManager?.AddOrUpdatePlayer(clientId, name);
            else if(op is SyncDictionaryOperation.Remove)
                _uiManager?.RemovePlayer(clientId);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            _uiManager?.ClearPlayerList();

            // Sync existing names to new client
            foreach (var pair in _playerNames)
            {
                _uiManager?.AddOrUpdatePlayer(pair.Key, pair.Value);
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            base.NetworkManager.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            base.NetworkManager.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
        }
        
        private void OnRemoteConnectionState(NetworkConnection connection, RemoteConnectionStateArgs rcs)
        {
            switch (rcs.ConnectionState)
            {
                case RemoteConnectionState.Started:
                    _playerCount++;
                    UpdatePlayerCountUI(_playerCount);
                    
                    if(!_playerNames.ContainsKey(connection.ClientId))
                        _playerNames[connection.ClientId] = $"Player_{_playerCount}";

                    if (GameManager.Instance.CurrentState is PreGameState &&
                        Game.TimeManager.Instance != null &&
                        !Game.TimeManager.Instance.IsTimerRunning)
                    {
                        if (connection.IsLocalClient)
                        {
                            _uiManager.ShowPreGameMessage();
                        }
                    }
                    break;
                case RemoteConnectionState.Stopped:
                    _playerNames.Remove(connection.ClientId);
                    _playerCount = Mathf.Max(0, _playerCount - 1);
                    UpdatePlayerCountUI(_playerCount);
                    break;
            }
        }
        
        [ObserversRpc(BufferLast = true)]
        private void UpdatePlayerCountUI(int count) => _uiManager?.UpdatePlayerCountText(count);
        
        public static string GetPlayerName(int clientId)
        {
            return _instance._playerNames.TryGetValue(clientId, out string result) ? result : string.Empty;
        }
        
        [Client]
        public static void SetName(string name) => _instance.ServerSetName(name);
        
        [ServerRpc(RequireOwnership = false)]
        private void ServerSetName(string name, NetworkConnection senderConnection = null)
        {
            if(senderConnection != null)
                _playerNames[senderConnection.ClientId] = name;
        }
    }
}
