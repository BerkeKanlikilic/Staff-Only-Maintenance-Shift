using _Scripts.UI;
using UnityEngine;
using FishNet.Object;
using PlayerInputManager = _Scripts.Player.PlayerInputManager;

namespace _Scripts.Interaction.Interactables
{
    public class MopTool : MonoBehaviour
    {
        [SerializeField] private float cleanRange = 1.5f;
        [SerializeField] private float holdDuration = 1.5f;
        [SerializeField] private LayerMask cleanableLayer;

        private float _holdTimer;
        private bool _isCleaning;
        private GameObject _target;
        private bool _wasUseHeldLastFrame;

        private void Update()
        {
            if (!PlayerInputManager.Instance || !PlayerInputManager.Instance.IsOwner)
                return;

            bool isUseHeld = PlayerInputManager.Instance.UseHeld;

            if (_isCleaning)
            {
                if (!Camera.main || !Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hit, cleanRange, cleanableLayer) ||
                    hit.collider.gameObject != _target)
                {
                    CancelCleaning();
                    return;
                }

                _holdTimer += Time.deltaTime;
                UIManager.Instance?.UpdateHoldProgress(_holdTimer, holdDuration);

                if (_holdTimer >= holdDuration)
                {
                    _isCleaning = false;
                    FinishCleaning();
                }

                if (_wasUseHeldLastFrame && !isUseHeld)
                {
                    CancelCleaning();
                }
            }

            _wasUseHeldLastFrame = isUseHeld;
        }

        public void TryUse()
        {
            if (_isCleaning || !Camera.main) return;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var hit, cleanRange, cleanableLayer))
            {
                if (hit.collider.CompareTag("WaterPuddle"))
                {
                    _target = hit.collider.gameObject;
                    _isCleaning = true;
                    _holdTimer = 0f;

                    UIManager.Instance?.StartHoldProgress(holdDuration);
                }
            }
        }

        private void FinishCleaning()
        {
            if (_target.TryGetComponent(out NetworkObject puddle))
            {
                GetComponentInParent<PlayerToolManager>()?.RequestPuddleClean(puddle);
            }

            UIManager.Instance?.HideHoldProgress();
            _target = null;
        }

        public void CancelCleaning()
        {
            if (!_isCleaning) return;

            _isCleaning = false;
            _target = null;
            _holdTimer = 0f;

            UIManager.Instance?.HideHoldProgress();
        }
    }
}