using System.Collections;
using Cinemachine;
using UnityEngine;

namespace CameraExtensions
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance { get; private set; }

        private CinemachineVirtualCamera _virtualCamera;
        private CinemachineBasicMultiChannelPerlin _noise;
        private Coroutine _shakeCoroutine;

        private void Awake()
        {
            Instance = this;
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _noise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        public void ShakeCamera(float intensity, float frequency, float duration)
        {
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
            }

            _shakeCoroutine = StartCoroutine(ShakeRoutine(intensity, frequency, duration));
        }

        public void SetTarget(Transform target)
        {
            _virtualCamera.Follow = target;
        }

        private IEnumerator ShakeRoutine(float intensity, float frequency, float duration)
        {
            _noise.m_AmplitudeGain = intensity;
            _noise.m_FrequencyGain = frequency;

            float elapsedTime = 0f;
            float startingIntensity = intensity;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                _noise.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, elapsedTime / duration);

                yield return null;
            }

            _noise.m_AmplitudeGain = 0f;
            _shakeCoroutine = null;
        }

        public void StopShake()
        {
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
                _noise.m_AmplitudeGain = 0f;
                _shakeCoroutine = null;
            }
        }
    }
}