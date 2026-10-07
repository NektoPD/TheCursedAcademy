using UnityEngine;

namespace Assets.Scripts.Utils
{
    [RequireComponent(typeof(ParticleSystem))]
    public class SmoothParticleDisabler : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        public void StopEmittingSmoothly() => _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}