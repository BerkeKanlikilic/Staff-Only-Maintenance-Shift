using FishNet.Connection;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Interaction
{
    public abstract class InteractableObject : NetworkBehaviour, IInteractable
    {
        [Header("Item Data")]
        [SerializeField] private InteractableItemData itemData;

        [Header("Interaction")]
        [SerializeField] private Transform interactionPoint;

        [Header("UI Elements")]
        [SerializeField] private Canvas whiteDotCanvas; // White dot canvas (world space)
        [SerializeField] private Canvas promptCanvas; // Prompt canvas (world space)
        [SerializeField] private TMP_Text interactionKeyText; // Text for interaction key (e.g., E)
        [SerializeField] private TMP_Text actionText; // Text for action (e.g., "Turn On")

        private bool _isPlayerNearby = false;
        public bool IsPlayerNearby => _isPlayerNearby;

        public string GetInteractionLabel() => itemData.interactionPrompt;
        public Vector3 GetInteractionPoint()
        {
            return interactionPoint ? interactionPoint.position : transform.position;
        }

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
            interactionKeyText.text = itemData.interactKey;
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
