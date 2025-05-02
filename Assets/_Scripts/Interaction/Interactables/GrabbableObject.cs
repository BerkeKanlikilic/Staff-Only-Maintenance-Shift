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

        private bool _isBeingHeld = false;
        public Rigidbody GetRigidbody() => _rigidbody;

        public override void OnStartServer()
        {
            base.OnStartServer();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void Interact(NetworkConnection interactor) { }

        public void AttachToPoint(Vector3 holdPointPosition, PlayerGrabController holder)
        {
            _rigidbody ??= GetComponent<Rigidbody>();

            _rigidbody.angularVelocity = Vector3.zero;
            _isBeingHeld = true;
            _holder = holder;

            if (_dynamicHoldPoint == null)
                _dynamicHoldPoint = new GameObject("DynamicHoldPoint").transform;

            _dynamicHoldPoint.position = holdPointPosition;

            if (_followRoutine != null)
                StopCoroutine(_followRoutine);

            _followRoutine = StartCoroutine(FollowHoldPoint());
        }

        private IEnumerator FollowHoldPoint()
        {
            while (_dynamicHoldPoint != null && _isBeingHeld)
            {
                HideWhiteDot();
                HidePrompt();
                
                Vector3 target = _dynamicHoldPoint.position;

                Vector3 direction = target - transform.position;
                Vector3 velocity = _rigidbody.linearVelocity;

                Vector3 spring = direction * springForce;
                Vector3 damp = -velocity * damping;

                Vector3 totalForce = spring + damp;

                _rigidbody.AddForce(totalForce, ForceMode.Force);

                if (direction.sqrMagnitude > maxGrabDistance * maxGrabDistance)
                {
                    Detach();
                    yield break;
                }

                yield return null;
            }
        }

        public void Detach()
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

            _isBeingHeld = false;

            if (_holder != null)
            {
                _holder.ForceRelease(this);
                _holder = null;
            }
        }
        
        public void UpdateHoldPoint(Vector3 pos)
        {
            if (_dynamicHoldPoint != null)
                _dynamicHoldPoint.position = pos;
        }

        public bool IsHeld => _isBeingHeld;
    }
    
}
