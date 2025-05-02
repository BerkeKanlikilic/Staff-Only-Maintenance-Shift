using _Scripts.Player;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Interaction
{
    public class TriggerCollider : NetworkBehaviour
    {
        [SerializeField] private InteractableObject interactableObject;
    
        private GameObject _clientObject;
    
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
    
            var playerController = other.GetComponent<PlayerController>();
            if (playerController == null || !playerController.IsOwner) return;
        
            interactableObject.SetPlayerNearby(true);
            interactableObject.ShowWhiteDot();
            PlayerInteraction.Instance.AddNearbyObject(interactableObject);

        }
    
        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
        
            var playerController = other.GetComponent<PlayerController>();
            if (playerController == null || !playerController.IsOwner) return;
        
            interactableObject.SetPlayerNearby(false);
            interactableObject.HideWhiteDot();
            interactableObject.HidePrompt();
            PlayerInteraction.Instance.RemoveNearbyObject(interactableObject);
        }
    }
}
