using System.Collections;
using System.Collections.Generic;
using HealthSystem;
using Items.BaseClass;
using UnityEngine;

namespace Items.ItemVariations.BeautiZone
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class BeautyZoneItemProjectile : ItemProjectile
    {
        [SerializeField] private float _damageTickInterval = 0.5f;
        [SerializeField] private ParticleSystem _particleEffectPrefab;
        [SerializeField] private int _initialPoolSize = 3;
        [SerializeField] private Transform _particleContainer;
        [SerializeField] private float _particleScaleOffset;

        private CircleCollider2D _circleCollider;
        private float _duration;
        private bool _isActive = false;
        private Coroutine _damageTickCoroutine;
        private ParticleSystem _activeParticle;

        private readonly Dictionary<int, Queue<ParticleSystem>> _particlePool = new();

        protected override void Awake()
        {
            base.Awake();
            _circleCollider = GetComponent<CircleCollider2D>();
            _circleCollider.isTrigger = true;
            _circleCollider.enabled = false;
        }

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

        public override void Initialize(float damage, Item owner)
        {
            base.Initialize(damage, owner);
            ClearHitEnemies();
        }

        public void SetRadius(float radius)
        {
            Transform.localScale = new Vector3(radius, radius);
            _circleCollider.radius = radius;
        }

        public void SetDuration(float duration)
        {
            _duration = duration;
        }

        public void Activate()
        {
            _isActive = true;
            _circleCollider.enabled = true;

            if (_damageTickCoroutine != null)
            {
                StopCoroutine(_damageTickCoroutine);
            }

            _damageTickCoroutine = StartCoroutine(DamageTick());
            PlayParticleEffect(_duration);

            StartCoroutine(DeactivateAfterDuration());
        }

        private void PlayParticleEffect(float duration)
        {
            if (_particleEffectPrefab == null) return;

            if (_activeParticle != null)
            {
                ReturnParticleToPool(_activeParticle);
                _activeParticle = null;
            }

            _activeParticle = GetParticleFromPool();
            _activeParticle.transform.position = transform.position;
            _activeParticle.transform.localScale = new Vector2(_circleCollider.radius + _particleScaleOffset, _circleCollider.radius + _particleScaleOffset);

            _activeParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = _activeParticle.main;
            main.duration = duration;

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

        private IEnumerator DeactivateAfterDuration()
        {
            yield return new WaitForSeconds(_duration);

            _isActive = false;
            _circleCollider.enabled = false;

            if (_damageTickCoroutine != null)
            {
                StopCoroutine(_damageTickCoroutine);
                _damageTickCoroutine = null;
            }
        }

        private IEnumerator DamageTick()
        {
            WaitForSeconds waitInterval = new WaitForSeconds(_damageTickInterval);

            while (_isActive)
            {
                yield return waitInterval;

                ClearHitEnemies();
            }
        }

        protected void OnTriggerStay2D(Collider2D collision)
        {
            if (!_isActive) return;

            if (collision.TryGetComponent(out IDamageable damageable) &&
                !collision.TryGetComponent(out CharacterLogic.Character character) &&
                !HitEnemies.Contains(damageable))
            {
                HitEnemies.Add(damageable);
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