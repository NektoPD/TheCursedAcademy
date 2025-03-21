using UnityEngine;

namespace Chunks
{
    public class ChunkMover : MonoBehaviour
    {
        [SerializeField] private float _chunkSize = 12f;
        [SerializeField] private CameraTracker _tracker;

        private int _countChunkInRow = 3;
        private float _sizeModificator = 2f;

        private void OnEnable()
        {
            _tracker.OnCameraMoved += HandleCameraMovement;
        }

        private void OnDisable()
        {
            _tracker.OnCameraMoved -= HandleCameraMovement;
        }

        private void HandleCameraMovement(Vector3 cameraPosition)
        {
            var mainCamera = Camera.main;

            Vector3 chunkPosition = transform.position;
            Vector3 delta = cameraPosition - chunkPosition;

            float cameraHeight = _sizeModificator * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            UpdateAxis(ref chunkPosition.x, delta.x, cameraWidth / _sizeModificator);
            UpdateAxis(ref chunkPosition.y, delta.y, cameraHeight / _sizeModificator);

            transform.position = chunkPosition;
        }

        private void UpdateAxis(ref float axis, float delta, float cameraOffset)
        {
            if (Mathf.Abs(delta) > _chunkSize + cameraOffset)
            {
                float direction = delta > 0 ? 1 : -1;
                axis += direction * _chunkSize * _countChunkInRow;
            }
        }
    }
}
