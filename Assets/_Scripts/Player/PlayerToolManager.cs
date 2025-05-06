using _Scripts.Interaction.Interactables;
using _Scripts.Player;
using _Scripts.UI;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerToolManager : NetworkBehaviour
{
    [SerializeField] private GameObject mopTool;
    [SerializeField] private GameObject mopPropPrefab;

    private readonly SyncVar<bool> _isMopActive = new();
    
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

    private void OnMopStateChanged(bool oldValue, bool newValue, bool asServer)
    {
        mopTool.SetActive(newValue);
        
        if (IsOwner)
            UIManager.Instance?.ToggleGrabUIPrompt(newValue, true);
    }
    
    public void TryUseActiveTool()
    {
        if (_isMopActive.Value && mopTool.TryGetComponent(out MopTool mop))
        {
            mop.TryUse();
        }
    }

    [Server]
    public void SetMopActive(bool active)
    {
        _isMopActive.Value = active;
    }

    [Client]
    public void UnequipMop()
    {
        if (!IsOwner) return;
        Debug.Log("[PlayerToolManager] UIManager.Instance is " + (UIManager.Instance ? "assigned ✅" : "null ❌"));
        RequestUnequipMop(PlayerGrabController.Instance?.HoldPoint.transform.position ?? transform.position);
        
        UIManager.Instance?.ToggleGrabUIPrompt(false, true);
    }
    
    [ServerRpc]
    private void RequestUnequipMop(Vector3 dropPosition)
    {
        SetMopActive(false);
        CmdSpawnDroppedMop(dropPosition);
    }

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

    
    [ServerRpc]
    public void RequestPuddleClean(NetworkObject puddle)
    {
        if (puddle != null)
            puddle.Despawn();
    }
}
