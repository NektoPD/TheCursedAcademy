using DG.Tweening;
using HealthSystem;
using UnityEngine;

namespace CharacterLogic.Abilities
{
    public class FireblastProjectile : AbilityProjectile
    {
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private float _searchRadius = 30f;
        [SerializeField] private float _spawnScaleDuration = 0.3f;
        [SerializeField] private float _despawnScaleDuration = 0.3f;
        [SerializeField] private float _speedMultiplier = 0.4f;

        private float _speed;
        private float _lifetime;
        private Transform _currentTarget;
        private Vector3 _targetScale;
        private bool _isDespawning;

        public override void Launch(Vector2 direction, float speed, float damage, float duration)
        {
            _speed = speed * _speedMultiplier;
            _lifetime = duration;
            _targetScale = transform.localScale;
            _isDespawning = false;

            transform.localScale = Vector3.zero;
            transform.DOScale(_targetScale, _spawnScaleDuration).SetEase(Ease.OutBack);

            base.Launch(direction, _speed, damage, duration);
            FindRandomTarget();

            float despawnDelay = _lifetime - _despawnScaleDuration;
            if (despawnDelay < 0f) despawnDelay = 0f;

            DOVirtual.DelayedCall(despawnDelay, StartDespawn);
        }

        private void StartDespawn()
        {
            if (this == null) return;
            _isDespawning = true;
            transform.DOScale(Vector3.zero, _despawnScaleDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(gameObject));
        }

        private void Update()
        {
            if (_isDespawning) return;

            if (_currentTarget == null || !_currentTarget.gameObject.activeInHierarchy)
            {
                FindRandomTarget();
                if (_currentTarget == null) return;
            }

            Vector2 dir = ((Vector2)_currentTarget.position - (Vector2)transform.position).normalized;
            Rb.velocity = dir * _speed;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        protected override void ApplyEffect(IDamageable target)
        {
            base.ApplyEffect(target);
            HitEnemies.Clear();
            FindRandomTarget();
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        private void FindRandomTarget()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _searchRadius, _enemyLayer);

            if (hits.Length == 0)
            {
                _currentTarget = null;
                return;
            }

            int startIndex = Random.Range(0, hits.Length);
            for (int i = 0; i < hits.Length; i++)
            {
                var candidate = hits[(startIndex + i) % hits.Length];
                if (candidate.transform != _currentTarget)
                {
                    _currentTarget = candidate.transform;
                    return;
                }
            }

            _currentTarget = hits[startIndex].transform;
        }
    }
}
