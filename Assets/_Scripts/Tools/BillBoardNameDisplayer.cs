using _Scripts.Network;
using FishNet.Connection;
using FishNet.Object;
using TMPro;
using UnityEngine;

namespace _Scripts.Tools
{
    public class BillBoardNameDisplayer : NetworkBehaviour
    {
        [SerializeField] private TextMeshPro _nameText;
    
        private Camera _camera;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsOwner)
            {
                _nameText.enabled = false;
                return;
            }
            SetName();
            PlayerInfoTracker.OnNameChange += PlayerNameTracker_OnNameChange;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            if (IsOwner) return;
            PlayerInfoTracker.OnNameChange -= PlayerNameTracker_OnNameChange;
        }

        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            if (IsOwner) return;
            SetName();
        }
    
        private void PlayerNameTracker_OnNameChange(int arg1, string arg2)
        {
            if (arg1 != Owner.ClientId) return;

            SetName();
        }

        private void Awake() => _camera = Camera.main;

        private void Update()
        {
            if (_camera == null) return;
        
            transform.LookAt(_camera.transform);
        
            // float distance = Vector3.Distance(transform.position, _camera.transform.position);
            // var tmpText = GetComponentInChildren<TMPro.TMP_Text>();
            // if (tmpText != null)
            // {
            //     tmpText.enabled = distance <= 3f;
            // }
        }

        private void SetName()
        {
            string result = null;
        
            if(Owner.IsValid)
                result = PlayerInfoTracker.GetPlayerName(Owner.ClientId);

            if (string.IsNullOrEmpty(result))
                result = "JohnDoe";
        
            _nameText.text = result;
        }
    }
}
