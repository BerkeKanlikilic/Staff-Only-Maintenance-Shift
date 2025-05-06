using System;
using System.Collections;
using _Scripts.Game.GameState;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Game
{
    public class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [SerializeField] private float gameDuration = 300f;
        
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
            
            SwitchState(new PreGameState());
        }
        
        public override void OnStopServer()
        {
            base.OnStopServer();
            if (Game.TimeManager.Instance != null)
                Game.TimeManager.Instance.OnTimeExpired -= HandleTimeExpired;
        }

        public void SwitchState(IGameState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public void OnExitDoorOpened()
        {
            if (!IsServerInitialized) return;
            SwitchState(new InGameState());
        }

        private void HandleTimeExpired()
        {
            Debug.Log("[GameManager] Timer expired. Handling end of game...");
        }
        
        private IEnumerator WaitForTimeManagerAndSubscribe()
        {
            yield return new WaitUntil(() => Game.TimeManager.Instance);

            Game.TimeManager.Instance.OnTimeExpired += HandleTimeExpired;
        }
        
        public static class GameState
        {
            public static bool IsGameFrozen = false;
        }
    }
}
