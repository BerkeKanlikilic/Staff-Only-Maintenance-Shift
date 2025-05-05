using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class UIManager : NetworkBehaviour
    {
        [Header("HUD")]
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private GameObject grabUiPrompt;
        [SerializeField] private GameObject hudUI;
        
        [Header("References")]
        [SerializeField] private TMP_Text playerCountText;
        [SerializeField] private Transform playerListContainer;
        [SerializeField] private PlayerListEntry playerListEntryPrefab;

        public static UIManager Instance { get; private set; }

        private readonly Dictionary<int, PlayerListEntry> _entryMap = new();
        private bool _isPaused;
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            if(!Instance)
                Instance = this;

            InitializeUI();
        }

        private void InitializeUI()
        {
            SetCursorState(false);
            ToggleGrabUIPrompt(false);
        }

        public void ToggleGrabUIPrompt(bool state)
        {
            SetGrabUIPrompt(state);
        }

        private void SetGrabUIPrompt(bool state)
        {
            if(grabUiPrompt)
                grabUiPrompt.SetActive(state);
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
    }
}
