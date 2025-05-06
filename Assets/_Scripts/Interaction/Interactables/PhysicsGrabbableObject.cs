using System;
using System.Collections;
using _Scripts.Player;
using _Scripts.UI;
using FishNet.Connection;
using UnityEngine;

namespace _Scripts.Interaction.Interactables
{
    public class PhysicsGrabbableObject : GrabbableObject
    {
        [Header("Grab Settings")]
        [SerializeField] private float springForce = 600f;
        [SerializeField] private float damping = 60f;
        [SerializeField] private float maxGrabDistance = 4f;

        private Rigidbody _rigidbody;
        private Transform _dynamicHoldPoint;
        private Coroutine _followRoutine;
        private PlayerGrabController _holder;

        public bool IsHeld => _holder != null;

        public override void OnStartServer()
        {
            base.OnStartServer();
            
            if (!IsServerInitialized)
            {
                enabled = false;
                return;
            }
            
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override bool TryGrab(PlayerGrabController grabber, Vector3 holdPosition)
        {
            if (IsHeld) return false;

            _holder = grabber;
            AttachToPoint(holdPosition);
            return true;
        }

        private void AttachToPoint(Vector3 holdPointPosition)
        {
            _rigidbody.angularVelocity = Vector3.zero;

            if (_dynamicHoldPoint == null)
                _dynamicHoldPoint = new GameObject("DynamicHoldPoint").transform;

            _dynamicHoldPoint.position = holdPointPosition;

            if (_followRoutine != null)
                StopCoroutine(_followRoutine);

            _followRoutine = StartCoroutine(FollowHoldPoint());
        }

        private IEnumerator FollowHoldPoint()
        {
            while (_dynamicHoldPoint != null && _holder != null)
            {
                Vector3 target = _dynamicHoldPoint.position;
                Vector3 direction = target - transform.position;
                Vector3 velocity = _rigidbody.linearVelocity;

                Vector3 spring = direction * springForce;
                Vector3 damp = -velocity * damping;
                Vector3 totalForce = spring + damp;

                _rigidbody.AddForce(totalForce, ForceMode.Force);

                if (direction.sqrMagnitude > maxGrabDistance * maxGrabDistance)
                {
                    Drop();
                    yield break;
                }

                yield return null;
            }
        }

        public override void Drop()
        {
            StopFollowing();
            
            _holder?.ForceRelease(this, true);
            _holder = null;
        }

        public override void Throw(Vector3 force)
        {
            Drop();
            _rigidbody.AddForce(force, ForceMode.Impulse);
        }

        public override void UpdateHoldPoint(Vector3 pos)
        {
            if (_dynamicHoldPoint)
                _dynamicHoldPoint.position = pos;
        }

        private void StopFollowing()
        {
            if (_followRoutine != null)
            {
                StopCoroutine(_followRoutine);
                _followRoutine = null;
            }

            if (_dynamicHoldPoint != null)
            {
                Destroy(_dynamicHoldPoint.gameObject);
                _dynamicHoldPoint = null;
            }
        }

        public override void ShowGrabPreview()
        {
            // Optional: add a particle or glow effect
            // This could be a highlight or UI prompt to indicate grab target
        }

        public override void OnGrabConfirmedClient()
        {
            // SFX
            UIManager.Instance?.ToggleGrabUIPrompt(true);
            // Debug.Log("Grab Confirmed.");
        }
        
        public override void OnDetachConfirmedClient()
        {
            // SFX
            UIManager.Instance?.ToggleGrabUIPrompt(false);
            // Debug.Log($"Detach Confirmed.");
        }
    }
}
