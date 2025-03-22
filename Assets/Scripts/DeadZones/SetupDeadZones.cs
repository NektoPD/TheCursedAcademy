using UnityEngine;

namespace DeadZones
{
    public class SetupDeadZones : MonoBehaviour
    {
        [SerializeField] private float _offset;
        [SerializeField] private DeadZone _topZone;
        [SerializeField] private DeadZone _bottomZone;
        [SerializeField] private DeadZone _leftZone;
        [SerializeField] private DeadZone _rightZone;

        private readonly float _heightMultiplier = 2f;

        private void Start()
        {
            Camera mainCamera = Camera.main;

            float cameraHeight = _heightMultiplier * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            _topZone.SetupZone(new Vector2(cameraWidth + _heightMultiplier * _offset, _offset), new Vector3(0, cameraHeight / _heightMultiplier + _offset, 0));
            _bottomZone.SetupZone(new Vector2(cameraWidth + _heightMultiplier * _offset, _offset), new Vector3(0, -cameraHeight / _heightMultiplier - _offset, 0));
            _leftZone.SetupZone(new Vector2(_offset, cameraHeight + _heightMultiplier * _offset), new Vector3(-cameraWidth / _heightMultiplier - _offset, 0, 0));
            _rightZone.SetupZone(new Vector2(_offset, cameraHeight + _heightMultiplier * _offset), new Vector3(cameraWidth / _heightMultiplier + _offset, 0, 0));
        }
    }
}
