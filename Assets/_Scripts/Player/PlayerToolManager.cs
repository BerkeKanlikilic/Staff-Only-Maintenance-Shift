using _Scripts.Game.Objective;
using _Scripts.Game.Objective.UI;
using _Scripts.Interaction.Interactables;
using _Scripts.UI;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace _Scripts.Player
{
    // Manages the player's tool logic (e.g. mop) in a networked multiplayer context.
    public class PlayerToolManager : NetworkBehaviour
    {
        [SerializeField] private GameObject mopTool;            // The mop tool attached to the player
        [SerializeField] private GameObject mopPropPrefab;      // The mop prop used when dropping it into the world

        private readonly SyncVar<bool> _isMopActive = new();    // Networked variable tracking whether the mop is currently active
    
        public bool IsToolActive => _isMopActive.Value;

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            _isMopActive.OnChange += OnMopStateChanged;
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            _isMopActive.OnChange -= OnMopStateChanged;
        }

        // Called when mop active state changes (on server or client)
        private void OnMopStateChanged(bool oldValue, bool newValue, bool asServer)
        {
            mopTool.SetActive(newValue);
        
            if (IsOwner)
                UIManager.Instance?.ToggleGrabUIPrompt(newValue, true);
        }
    
        // Called when the use button is pressed and a tool is active
        public void TryUseActiveTool()
        {
            if (_isMopActive.Value && mopTool.TryGetComponent(out MopTool mop))
            {
                mop.TryUse();
            }
        }

        // Server-side method to activate or deactivate mop
        [Server]
        public void SetMopActive(bool active)
        {
            _isMopActive.Value = active;
        }

        // Called by the client to unequip mop (requests drop on server)
        [Client]
        public void UnequipMop()
        {
            if (!IsOwner) return;
            RequestUnequipMop(PlayerGrabController.Instance?.HoldPoint.transform.position ?? transform.position);
        
            UIManager.Instance?.ToggleGrabUIPrompt(false, true);
        }
    
        // ServerRpc to unequip the mop and spawn its dropped version in the world
        [ServerRpc]
        private void RequestUnequipMop(Vector3 dropPosition)
        {
            SetMopActive(false);
            CmdSpawnDroppedMop(dropPosition);
        }

        // Server-side spawning of dropped mop
        [Server]
        private void CmdSpawnDroppedMop(Vector3 dropPosition)
        {
            if (mopPropPrefab == null)
            {
                Debug.LogError("Mop prop prefab is not assigned.");
                return;
            }

            GameObject mopInstance = Instantiate(mopPropPrefab, dropPosition, Quaternion.identity);

            NetworkObject netObj = mopInstance.GetComponent<NetworkObject>();

            if (netObj != null)
            {
                base.NetworkManager.ServerManager.Spawn(netObj, Owner);
            }
            else
            {
                Debug.LogError("Spawned mop object is missing a NetworkObject.");
            }
        }

        // ServerRpc request to clean a puddle object (called when mop is used)
        [ServerRpc]
        public void RequestPuddleClean(NetworkObject puddle)
        {
            if (puddle != null)
            {
                if (puddle.TryGetComponent(out Puddle puddleComponent))
                {
                    puddleComponent.Clean();
                }
            
                if (puddle.TryGetComponent(out WorldIndicator indicator))
                {
                    indicator.SetVisible(false);
                }

                puddle.Despawn();
            }
        }

    }
}
