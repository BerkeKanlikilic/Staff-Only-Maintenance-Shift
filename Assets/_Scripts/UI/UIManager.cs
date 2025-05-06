using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIManager : NetworkBehaviour
    {
        [Header("HUD")]
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private GameObject grabUiPrompt;
        [SerializeField] private GameObject useUiPrompt;
        [SerializeField] private GameObject hudUI;
        [SerializeField] private TMP_Text timerText;

        [Header("Cleaning UI")]
        [SerializeField] private GameObject holdProgressUI;
        [SerializeField] private Image holdProgressImage;
        
        [Header("References")]
        [SerializeField] private TMP_Text playerCountText;
        [SerializeField] private Transform playerListContainer;
        [SerializeField] private PlayerListEntry playerListEntryPrefab;

        public static UIManager Instance { get; private set; }

        private readonly Dictionary<int, PlayerListEntry> _entryMap = new();
        private bool _isPaused;
        private Coroutine _hideProgressRoutine;
        
        private void Awake()
        {
            if (!Instance)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }
        
        private void Start()
        {
            StartCoroutine(WaitAndSubscribeToTimeManager());
        }

        private IEnumerator WaitAndSubscribeToTimeManager()
        {
            yield return new WaitUntil(() => Game.TimeManager.Instance);
            Game.TimeManager.Instance.OnTimeUpdated += UpdateTimerText;
        }

        
        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!IsOwner) return;

            InitializeUI();
        }

        private void InitializeUI()
        {
            SetCursorState(false);
            ToggleGrabUIPrompt(false);
        }

        public void ToggleGrabUIPrompt(bool state, bool hasUse = false)
        {
            SetGrabUIPrompt(state, hasUse);
        }

        private void SetGrabUIPrompt(bool state, bool hasUse)
        {
            if(!hasUse)
            {
                if (grabUiPrompt)
                    grabUiPrompt.SetActive(state);
            }
            else
            {
                if(useUiPrompt)
                    useUiPrompt.SetActive(state);
            }
        }

        public void TogglePauseMenu()
        {
            SetPauseMenu(!_isPaused);
        }

        private void SetPauseMenu(bool state)
        {
            _isPaused = state;
            if(pauseMenuUI)
                pauseMenuUI.SetActive(state);
            
            if(hudUI)
                hudUI.SetActive(!state);
            
            SetCursorState(_isPaused);
        }

        private static void SetCursorState(bool paused)
        {
            Cursor.visible = paused;
            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void UpdatePlayerCountText(int count)
        {
            if(playerCountText)
                playerCountText.text = $"Players: {count}";
        }

        public void ClearPlayerList()
        {
            foreach (var entry in _entryMap.Values.Where(entry => entry != null))
            {
                Destroy(entry.gameObject);
            }

            _entryMap.Clear();
        }

        public void AddOrUpdatePlayer(int clientId, string playerName)
        {
            if (_entryMap.TryGetValue(clientId, out var entry))
            {
                entry.SetName(playerName);
            }
            else
            {
                var newEntry = Instantiate(playerListEntryPrefab, playerListContainer);
                newEntry.SetName(playerName);
                _entryMap[clientId] = newEntry;
            }
        }

        public void RemovePlayer(int clientId)
        {
            if (!_entryMap.TryGetValue(clientId, out var entry)) return;
            
            if(entry)
                Destroy(entry.gameObject);
            _entryMap.Remove(clientId);
        }

        public void UpdateTimerText(float seconds)
        {
            if (!timerText) return;

            TimeSpan time = TimeSpan.FromSeconds(seconds);
            timerText.text = $"{time.Minutes:D2}:{time.Seconds:D2}";
        }
        
        public void ShowPreGameMessage()
        {
            timerText.text = "Timer will start when you open the door!";
        }

        public void StartHoldProgress(float duration)
        {
            if(holdProgressUI) holdProgressUI.SetActive(true);
            if (holdProgressImage) holdProgressImage.fillAmount = 0f;
        }

        public void UpdateHoldProgress(float currentTime, float holdDuration)
        {
            if (holdProgressImage)
                holdProgressImage.fillAmount = Mathf.Clamp01(currentTime / holdDuration);
        }

        public void HideHoldProgress()
        {
            if(holdProgressUI) holdProgressUI.SetActive(false);
        }
    }
}
