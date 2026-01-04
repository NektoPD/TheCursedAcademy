using System.Collections;
using Cinemachine;
using UnityEngine;

namespace CameraExtensions
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraDeathZoom : MonoBehaviour
    {
        public static CameraDeathZoom Instance { get; private set; }

        [SerializeField] private float _deathZoomSize = 3.5f;   // во что зумим (ортографический размер)
        [SerializeField] private float _zoomDuration = 0.6f;    // длительность зума (реальное время)

        private CinemachineVirtualCamera _virtualCamera;
        private Coroutine _zoomCoroutine;
        private float _defaultOrthoSize;

        private void Awake()
        {
            Instance = this;
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _defaultOrthoSize = _virtualCamera.m_Lens.OrthographicSize;
        }

        public void SetTarget(Transform target)
        {
            _virtualCamera.Follow = target;
        }

        public void PlayDeathZoom()
        {
            // if (_zoomCoroutine != null)
            //     StopCoroutine(_zoomCoroutine);
            //
            // _zoomCoroutine = StartCoroutine(ZoomRoutine(_defaultOrthoSize, _deathZoomSize, _zoomDuration));
        }

        public void ResetZoom(float duration = 0.2f)
        {
            if (_zoomCoroutine != null)
                StopCoroutine(_zoomCoroutine);

            _zoomCoroutine = StartCoroutine(ZoomRoutine(_virtualCamera.m_Lens.OrthographicSize, _defaultOrthoSize, duration));
        }

        private IEnumerator ZoomRoutine(float from, float to, float duration)
        {
            float t = 0f;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float k = duration <= 0f ? 1f : Mathf.Clamp01(t / duration);
                _virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(from, to, k);
                yield return null;
            }

            _virtualCamera.m_Lens.OrthographicSize = to;
            _zoomCoroutine = null;
        }
    }
}
