using System.Collections;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace _Scripts.Interaction.Interactables
{
    // Base class for a networked door that can open/close with interaction.
    public class Door : InteractableObject
    {
        [Header("Settings")]
        [SerializeField] private Transform doorTransform;
        [SerializeField] private float openTime = 1f; // in seconds
        [SerializeField] private float openAngle = 90f; // in degrees
        [SerializeField] private bool mirror = false;

        private bool _isOn = false;
        private Coroutine _doorRoutine;
        private Quaternion _closedRotation;
        private Quaternion _openRotation;

        public bool GetIsOpen() => _isOn;
        
        public override void OnStartNetwork()
        {
            base.OnStartNetwork();

            _closedRotation = doorTransform.localRotation;

            float angle = mirror ? -openAngle : openAngle;
            _openRotation = _closedRotation * Quaternion.Euler(0, angle, 0);
        }

        public override void Interact(NetworkConnection interactor)
        {
            if(!IsServerStarted) return;

            _isOn = !_isOn;
            RpcUpdateDoor(_isOn);
        }

        [ObserversRpc]
        private void RpcUpdateDoor(bool state)
        {
            UpdateDoorVisual(state);
        }

        private void UpdateDoorVisual(bool state)
        {
            if(_doorRoutine != null)
                StopCoroutine(_doorRoutine);

            _doorRoutine = StartCoroutine(RotateDoor(state ? _openRotation : _closedRotation));
        }

        private IEnumerator RotateDoor(Quaternion targetRotation)
        {
            Quaternion startRotation = doorTransform.localRotation;
            
            float angleRemaining = Quaternion.Angle(startRotation, targetRotation);
            float totalAngle = Quaternion.Angle(_closedRotation, _openRotation);
            
            float ratio = Mathf.Clamp01(angleRemaining / totalAngle);
            float duration = Mathf.Max(0.01f, openTime * ratio);
            
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                doorTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
                yield return null;
            }
            
            doorTransform.localRotation = targetRotation;
        }
    }
}
