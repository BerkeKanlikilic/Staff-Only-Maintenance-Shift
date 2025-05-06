using System;
using System.Collections;
using _Scripts.Game.GameState;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Game
{
    // Handles game flow and state transitions (pre-game, in-game, end)
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [SerializeField] private float gameDuration = 300f; // Default 5 minutes
        
        public float GameDuration => gameDuration;
        public IGameState CurrentState => _currentState;
        
        private IGameState _currentState;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            StartCoroutine(WaitForTimeManagerAndSubscribe());
            
            // Start in Pre-Game
            SwitchState(new PreGameState());
        }
        
        public override void OnStopServer()
        {
            base.OnStopServer();
            if (Game.TimeManager.Instance != null)
                Game.TimeManager.Instance.OnTimeExpired -= HandleTimeExpired;
        }

        // Switch to a new game state
        public void SwitchState(IGameState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        // Called when exit door is opened — transition to InGame
        public void OnExitDoorOpened()
        {
            if (!IsServerInitialized) return;
            SwitchState(new InGameState());
        }

        private void HandleTimeExpired()
        {
            Debug.Log("[GameManager] Timer expired. Handling end of game...");
            // TODO: Add win/lose state change if needed
        }
        
        private IEnumerator WaitForTimeManagerAndSubscribe()
        {
            yield return new WaitUntil(() => Game.TimeManager.Instance);

            Game.TimeManager.Instance.OnTimeExpired += HandleTimeExpired;
        }
        
        // Shared static flag used to freeze input/gameplay globally
        public static class GameState
        {
            public static bool IsGameFrozen = false;
        }
    }
}
