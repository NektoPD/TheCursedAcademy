using UnityEngine;
using HealthSystem;
using Items.BaseClass;
using System.Collections;
using System.Collections.Generic;

namespace Items.ItemVariations
{
    public class PenProjectile : ItemProjectile
    {
        [SerializeField] private ParticleSystem _particleEffectPrefab;
        [SerializeField] private float _particleOffset = 0.5f;
        [SerializeField] private int _initialPoolSize = 3;
        [SerializeField] private float _leftRotationAngle = 180f;
        [SerializeField] private float _rightRotationAngle = 0f;

        [SerializeField] private Transform _particleContainer;

        private readonly Dictionary<int, Queue<ParticleSystem>> _particlePool = new();

        private ParticleSystem _activeParticle;

        private void InitializeParticlePool(int initialSize)
        {
            int prefabID = _particleEffectPrefab.GetInstanceID();

            if (!_particlePool.ContainsKey(prefabID))
            {
                _particlePool[prefabID] = new Queue<ParticleSystem>();

                for (int i = 0; i < initialSize; i++)
                {
                    CreateNewParticleInPool(prefabID);
                }
            }
        }

        private void CreateNewParticleInPool(int prefabID)
        {
            ParticleSystem newParticle = Instantiate(_particleEffectPrefab, _particleContainer);
            newParticle.gameObject.SetActive(false);
            _particlePool[prefabID].Enqueue(newParticle);
        }

        private ParticleSystem GetParticleFromPool()
        {
            int prefabID = _particleEffectPrefab.GetInstanceID();

            if (!_particlePool.ContainsKey(prefabID))
            {
                InitializeParticlePool(_initialPoolSize);
            }

            ParticleSystem particle;

            if (_particlePool[prefabID].Count > 0)
            {
                particle = _particlePool[prefabID].Dequeue();
            }
            else
            {
                CreateNewParticleInPool(prefabID);
                particle = _particlePool[prefabID].Dequeue();
            }

            particle.gameObject.SetActive(true);

            return particle;
        }

        private void ReturnParticleToPool(ParticleSystem particle)
        {
            if (particle == null) return;

            int prefabID = _particleEffectPrefab.GetInstanceID();

            particle.gameObject.SetActive(false);

            if (!_particlePool.ContainsKey(prefabID))
            {
                InitializeParticlePool(_initialPoolSize);
            }

            _particlePool[prefabID].Enqueue(particle);
        }

        public void PlayParticleEffect(float duration, bool isMovingLeft)
        {
            if (_particleEffectPrefab == null) return;

            if (_activeParticle != null)
            {
                ReturnParticleToPool(_activeParticle);
                _activeParticle = null;
            }

            _activeParticle = GetParticleFromPool();

            float xOffset = isMovingLeft ? -_particleOffset : _particleOffset;
            Vector3 particlePosition = new Vector3(
                transform.position.x + xOffset,
                transform.position.y,
                transform.position.z
            );

            _activeParticle.transform.position = particlePosition;
            _activeParticle.transform.rotation = Quaternion.Euler(
                0,
                0,
                isMovingLeft ? _leftRotationAngle : _rightRotationAngle
            );

            _activeParticle.Clear();
            _activeParticle.Play();

            StartCoroutine(ReturnParticleAfterDuration(_activeParticle, duration));
        }

        private IEnumerator ReturnParticleAfterDuration(ParticleSystem particle, float duration)
        {
            yield return new WaitForSeconds(duration);

            if (particle != null)
            {
                ReturnParticleToPool(particle);

                if (_activeParticle == particle)
                {
                    _activeParticle = null;
                }
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out IDamageable damageable) && HitEnemies.Add(damageable))
            {
                damageable?.TakeDamage(Damage);
            }
        }

        private void OnDisable()
        {
            if (_activeParticle != null)
            {
                ReturnParticleToPool(_activeParticle);
                _activeParticle = null;
            }

            StopAllCoroutines();
        }
    }
}