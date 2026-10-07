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
        private float _initialAmplitude;
        private float _initialFrequency;

        private void Awake()
        {
            Instance = this;
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _noise = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (_noise != null)
            {
                _initialAmplitude = _noise.m_AmplitudeGain;
                _initialFrequency = _noise.m_FrequencyGain;
            }
        }

        private void OnDisable()
        {
            StopShake();
            if (Instance == this)
                Instance = null;
        }

        private void OnEnable()
        {
            Instance = this;
        }

        public void ShakeCamera(float intensity, float frequency, float duration)
        {
            if (_noise == null || intensity <= 0f || duration <= 0f)
                return;

            if (_shakeCoroutine != null)
            {
                if (intensity < _noise.m_AmplitudeGain)
                    return;

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

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float t = Mathf.Clamp01(elapsedTime / duration);
                float falloff = (1f - t) * (1f - t);

                _noise.m_AmplitudeGain = intensity * falloff;

                yield return null;
            }

            RestoreNoise();
            _shakeCoroutine = null;
        }

        public void StopShake()
        {
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
                _shakeCoroutine = null;
            }

            RestoreNoise();
        }

        private void RestoreNoise()
        {
            if (_noise == null)
                return;

            _noise.m_AmplitudeGain = _initialAmplitude;
            _noise.m_FrequencyGain = _initialFrequency;
        }
    }
}