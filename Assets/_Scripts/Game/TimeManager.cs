using System;
using _Scripts.UI;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Game
{
    public class TimeManager : NetworkBehaviour
    {
        public static TimeManager Instance { get; private set; }

        public event Action<float> OnTimeUpdated;
        public event Action OnTimeExpired;

        private float _remainingTime;
        private bool _timerRunning;
        
        private float _secondTimer = 0f;
        public bool IsTimerRunning => _timerRunning;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            Instance = this;
        }

        public void StartTimer(float duration)
        {
            if (!IsServerInitialized) return;
            
            _remainingTime = duration;
            _timerRunning = true;
            _secondTimer = 0f;
            
            RpcUpdateTime(Mathf.FloorToInt(_remainingTime));
            OnTimeUpdated?.Invoke(_remainingTime);
        }

        private void Update()
        {
            if (!IsServerInitialized || !_timerRunning) return;

            _secondTimer += Time.deltaTime;

            if (_secondTimer >= 1f)
            {
                _secondTimer = 0f;

                _remainingTime -= 1f;

                int secondsRemaining = Mathf.Max(0, Mathf.FloorToInt(_remainingTime));
                RpcUpdateTime(secondsRemaining);
                OnTimeUpdated?.Invoke(secondsRemaining);

                if (_remainingTime <= 0f)
                {
                    _timerRunning = false;
                    OnTimeExpired?.Invoke();
                    RpcGameFailed();
                    Debug.Log("[TimeManager] Timer expired.");
                }
            }
        }

        [ObserversRpc]
        private void RpcUpdateTime(float time)
        {
            OnTimeUpdated?.Invoke(time);
        }
        
        [ObserversRpc]
        private void RpcGameFailed()
        {
            UIManager.Instance?.ShowGameEndScreen("You failed...");
        }
    }
}
