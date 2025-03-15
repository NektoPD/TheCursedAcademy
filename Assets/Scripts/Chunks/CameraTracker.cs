using System;
using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    [SerializeField] private float _positionThreshold = 0.1f;

    private Vector3 _lastPosition;

    public event Action<Vector3> OnCameraMoved;

    private void Start()
    {
        _lastPosition = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _lastPosition) > _positionThreshold)
        {
            _lastPosition = transform.position;
            OnCameraMoved?.Invoke(_lastPosition);
        }
    }
}
