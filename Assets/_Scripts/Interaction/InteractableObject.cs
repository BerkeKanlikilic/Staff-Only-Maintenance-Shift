using FishNet.Connection;
using FishNet.Object;
using TMPro;
using UnityEngine;

namespace _Scripts.Interaction
{
    public abstract class InteractableObject : NetworkBehaviour, IInteractable
    {
        [Header("Item Data")]
        [SerializeField] private InteractableItemData itemData;

        [Header("UI Elements")]
        [SerializeField] private Canvas whiteDotCanvas; // White dot canvas (world space)
        [SerializeField] private Canvas promptCanvas; // Prompt canvas (world space)
        [SerializeField] private TMP_Text itemNameText; // Text for item name
        [SerializeField] private TMP_Text actionText; // Text for action (e.g., "F to Turn On")

        private bool _isPlayerNearby = false;
        public bool IsPlayerNearby => _isPlayerNearby;

        public string GetInteractionLabel() => itemData.interactionPrompt;

        public virtual bool CanInteract(NetworkConnection interactor) => true;
    
        public abstract void Interact(NetworkConnection interactor);

        private void Start()
        {
            SetCanvasState(whiteDotCanvas, false);
            SetCanvasState(promptCanvas, false);
        }
        
        public void ShowWhiteDot()
        {
            if (_isPlayerNearby)
            {
                SetCanvasState(whiteDotCanvas, true);
            }
        }
        
        public void HideWhiteDot() => SetCanvasState(whiteDotCanvas, false);
        
        public void ShowPrompt()
        {
            HideWhiteDot();
            SetCanvasState(promptCanvas, true);
            itemNameText.text = itemData.itemName;
            actionText.text = itemData.interactionPrompt;
        }
        
        public void HidePrompt()
        {
            SetCanvasState(promptCanvas, false);
            if (_isPlayerNearby)
            {
                ShowWhiteDot();
            }
        }
        
        public void SetPlayerNearby(bool isNearby)
        {
            _isPlayerNearby = isNearby;
            if (!isNearby)
            {
                HideWhiteDot();
                // HidePrompt();
            }
        }

        private void SetCanvasState(Canvas canvas, bool state)
        {
            if (canvas != null && canvas.gameObject.activeSelf != state)
            {
                canvas.gameObject.SetActive(state);
            }
        }
    }
}
