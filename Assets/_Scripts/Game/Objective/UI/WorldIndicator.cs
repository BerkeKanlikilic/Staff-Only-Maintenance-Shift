using UnityEngine;

namespace _Scripts.Game.Objective.UI
{
    public class WorldIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject indicatorPrefab;
        [SerializeField] private Vector3 offset = Vector3.up * 2f;

        private Transform _mainCamera;
        private GameObject _spawnedIndicator;
        private bool _visible = false;

        private void Start()
        {
            _mainCamera = Camera.main.transform;

            if (indicatorPrefab != null)
            {
                _spawnedIndicator = Instantiate(indicatorPrefab, transform.position + offset, Quaternion.identity);
                _spawnedIndicator.layer = LayerMask.NameToLayer("AlwaysVisible");
                _spawnedIndicator.SetActive(_visible);
            }

            WorldIndicatorRegistry.Register(this);
        }

        private void LateUpdate()
        {
            if (_spawnedIndicator != null && _mainCamera != null && _visible)
            {
                _spawnedIndicator.transform.position = transform.position + offset;
                _spawnedIndicator.transform.forward = _mainCamera.forward;
            }
        }

        public void SetVisible(bool visible)
        {
            _visible = visible;
            if (_spawnedIndicator != null)
                _spawnedIndicator.SetActive(visible);
        }

        private void OnDestroy()
        {
            if (_spawnedIndicator != null)
                Destroy(_spawnedIndicator);

            WorldIndicatorRegistry.Unregister(this);
        }
    }
}