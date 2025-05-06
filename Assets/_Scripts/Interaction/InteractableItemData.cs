using UnityEngine;

namespace _Scripts.Interaction
{
    // Holds UI-related strings for an interactable object
    [CreateAssetMenu(fileName = "InteractableItemData", menuName = "Scriptable Objects/InteractableItemData")]
    public class InteractableItemData : ScriptableObject
    {
        public string interactKey;              // Key prompt (e.g., "E")
        public string interactionPrompt;        // Main interaction text (e.g., "Open Door")
        public string secondInteractionPrompt;  // Optional secondary prompt
    }
}
