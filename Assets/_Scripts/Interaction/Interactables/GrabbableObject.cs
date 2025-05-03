using System.Collections;
using _Scripts.Player;
using FishNet.Connection;
using UnityEngine;

namespace _Scripts.Interaction.Interactables
{
    public class GrabbableObject : InteractableObject
    {
        [Header("Grab Settings")]
        [SerializeField] private float springForce = 600f;
        [SerializeField] private float damping = 60f;
        [SerializeField] private float maxGrabDistance = 4f;

        private Rigidbody _rigidbody;
        private Transform _dynamicHoldPoint;
        private Coroutine _followRoutine;
        private PlayerGrabController _holder;
        private Collider _collider;

        public bool IsHeld => _holder != null;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public bool TryGrab(PlayerGrabController grabber, Vector3 holdPosition)
        {
            if (IsHeld) return false;

            _holder = grabber;
            AttachToPoint(holdPosition);
            return true;
        }

        public void AttachToPoint(Vector3 holdPointPosition)
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

        public void Drop()
        {
            StopFollowing();
            _holder?.ForceRelease(this, true);
            _holder = null;
        }

        public void Throw(Vector3 force)
        {
            Drop();
            _rigidbody.AddForce(force, ForceMode.Impulse);
        }

        public void UpdateHoldPoint(Vector3 pos)
        {
            if (_dynamicHoldPoint != null)
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

        public void ShowGrabPreview()
        {
            // Optional: add a particle or glow effect
            // This could be a highlight or UI prompt to indicate grab target
        }

        public void OnGrabConfirmedClient()
        {
            // Optional: animate to hand or play sound
            // Example: play an attach sound or enable outline
            Debug.Log("Grab Confirmed.");
            _collider = GetComponent<MeshCollider>();
            if(_collider) _collider.enabled = false; 
        }
        
        public void OnDropConfirmedClient()
        {
            if (_collider == null)
                _collider = GetComponent<MeshCollider>();
    
            if (_collider != null)
                _collider.enabled = true;
        }

        public Rigidbody GetRigidbody() => _rigidbody;
        public override void Interact(NetworkConnection interactor) { }
    }
}
