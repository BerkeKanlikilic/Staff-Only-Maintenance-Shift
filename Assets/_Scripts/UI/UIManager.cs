using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private GameObject hudUI;
        
        [Header("References")]
        [SerializeField] private TMP_Text playerCountText;
        [SerializeField] private Transform playerListContainer;
        [SerializeField] private PlayerListEntry playerListEntryPrefab;

        public bool IsPaused => _isPaused;

        private readonly Dictionary<int, PlayerListEntry> _entryMap = new();
        private bool _isPaused = false;
        
        public void UpdatePlayerCountText(int count) => playerCountText.text = $"Players: {count}";

        private void Start()
        {
            SetCursorState(false);
        }

        public void TogglePauseMenu()
        {
            SetPauseMenu(!_isPaused);
        }

        public void SetPauseMenu(bool state)
        {
            _isPaused = state;
            pauseMenuUI.SetActive(state);
            hudUI?.SetActive(!state);
            SetCursorState(_isPaused);
        }

        private void SetCursorState(bool paused)
        {
            Cursor.visible = paused;
            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void ClearPlayerList()
        {
            foreach (var entry in _entryMap.Values)
            {
                Destroy(entry.gameObject);
            }
            
            _entryMap.Clear();
        }

        public void AddOrUpdatePlayer(int clientId, string name)
        {
            if (_entryMap.TryGetValue(clientId, out var entry))
                entry.SetName(name);
            else
            {
                var newEntry = Instantiate(playerListEntryPrefab, playerListContainer);
                newEntry.SetName(name);
                _entryMap[clientId] = newEntry;
            }
        }

        public void RemovePlayer(int clientId)
        {
            if (_entryMap.TryGetValue(clientId, out var entry))
            {
                Destroy(entry.gameObject);
                _entryMap.Remove(clientId);
            }
        }
    }
}
