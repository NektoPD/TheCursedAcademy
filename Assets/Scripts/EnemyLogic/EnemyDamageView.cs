using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace EnemyLogic
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyDamageView : MonoBehaviour
    {
        [SerializeField] private Color _damageColor = new(1f, 0.5f, 0.5f);
        [SerializeField, Range(0f, 1f)] private float _flashHoldPart = 0.35f;

        [Header("Poison")]
        [SerializeField] private Color _poisonColor = new(0.2f, 1f, 0.2f);
        
        [Header("Hit Impulse (Jump + Knockback)")]
        [SerializeField] private float _impulseDuration = 0.12f;
        [SerializeField] private float _knockbackDistance = 0.25f;
        [SerializeField] private float _jumpHeight = 0.12f;
        [SerializeField] private Ease _impulseEase = Ease.OutQuad;

        [Header("Impulse Limits")]
        [SerializeField] private float _impulseCooldown = 0.35f;

        [SerializeField] private float _minDistanceForFullKnockback = 0.8f;
        [SerializeField] private float _minKnockbackMultiplier = 0.15f;
        [SerializeField] private bool _disableHorizontalKnockbackWhenVeryClose = true;
        [SerializeField] private float _veryCloseDistance = 0.25f;

        [SerializeField] private float _impulseCooldownNormal = 0.35f;
        [SerializeField] private float _impulseCooldownBoss = 0.7f;

        [Header("Hit Squash & Stretch")]
        [SerializeField] private bool _useSquash = true;
        [SerializeField] private float _squashAmount = 0.18f;
        [SerializeField] private float _squashDuration = 0.16f;

        [Header("Damage Numbers")]
        [SerializeField] private TMP_FontAsset _damageFont;
        [SerializeField] private Color _damageNumberColor = Color.white;
        
        [SerializeField] private float _damageNumberStartHeight = 0.55f;
        [SerializeField] private float _damageNumberArcHeight = 0.45f;
        [SerializeField] private float _damageNumberHorizontalSpread = 0.35f;
        [SerializeField] private float _damageNumberEndDrop = 0.15f;
        [SerializeField] private float _damageNumberDuration = 0.75f;
        [SerializeField] private float _damageNumberPopDuration = 0.1f;
        [SerializeField] private int _damageNumberSortingOrder = 10;

        private float _damageNumberFontSize = 5f;
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;
        private Coroutine _coroutine;
        private Tween _poisonTween;
        private float _poisonStrength;
        private float _flashStrength;
        private bool _isPoisoned;

        private Tween _impulseTween;
        private Tween _squashTween;
        private Vector3 _originalScale;

        private float _nextImpulseTime;
        private int _bossEnemyId = 1000;
        
        public int EnemyId { get; private set; }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _originalScale = transform.localScale;
        }

        private void OnEnable()
        {
            _originalColor = _spriteRenderer.color;
            _nextImpulseTime = 0f;

            if (_isPoisoned)
                StartPoisonPulse();
        }

        private void OnDisable()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = null;
            _poisonTween?.Kill();
            _poisonTween = null;
            _poisonStrength = 0f;
            _flashStrength = 0f;
            _spriteRenderer.color = _originalColor;

            if (!gameObject.activeInHierarchy)
                _isPoisoned = false;

            _impulseTween?.Kill();

            _squashTween?.Kill();
            transform.localScale = _originalScale;
            _nextImpulseTime = 0f;
        }

        public void Initialize(int enemyId)
        {
            EnemyId = enemyId;

            _impulseCooldown = EnemyId >= _bossEnemyId ? _impulseCooldownBoss : _impulseCooldownNormal;
        }

        public void StartFlash(float duration)
        {
            RestartFlash(duration);
            ApplySquash();
        }

        public void StartFlash(float duration, Vector2 hitFromWorldPos)
        {
            RestartFlash(duration);
            ApplySquash();
            ApplyHitImpulse(hitFromWorldPos);
        }

        public void StartPoisonPulse()
        {
            _isPoisoned = true;
            if (!isActiveAndEnabled || _poisonTween != null && _poisonTween.IsActive())
                return;

            _poisonTween = DOVirtual.Float(0.15f, 0.7f, 0.45f, strength =>
                {
                    _poisonStrength = strength;
                    UpdateTint();
                })
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void StopPoisonPulse()
        {
            _isPoisoned = false;
            _poisonTween?.Kill();
            _poisonTween = null;
            _poisonStrength = 0f;

            if (isActiveAndEnabled)
                UpdateTint();
        }

        public void SetBaseColor(Color color)
        {
            _originalColor = color;
            if (isActiveAndEnabled)
                UpdateTint();
            else
                _spriteRenderer.color = color;
        }

        private void UpdateTint()
        {
            Color tint = Color.Lerp(_originalColor, _poisonColor, _poisonStrength);
            tint.a = _originalColor.a;
            _spriteRenderer.color = Color.Lerp(tint, _damageColor, _flashStrength);
        }

        public void ShowDamageNumber(float damage)
        {
            if (_damageFont == null || damage <= 0f)
                return;

            GameObject damageObject = new GameObject("EnemyDamageNumber");
            TextMeshPro damageText = damageObject.AddComponent<TextMeshPro>();
            damageText.font = _damageFont;
            damageText.fontSize = _damageNumberFontSize;
            damageText.alignment = TextAlignmentOptions.Center;
            damageText.enableWordWrapping = false;
            damageText.overflowMode = TextOverflowModes.Overflow;
            damageText.text = FormatDamage(damage);
            damageText.color = _damageNumberColor;

            MeshRenderer renderer = damageObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerID = _spriteRenderer.sortingLayerID;
                renderer.sortingOrder = _spriteRenderer.sortingOrder + _damageNumberSortingOrder;
            }

            Vector3 start = transform.position + new Vector3(
                Random.Range(-0.08f, 0.08f),
                _damageNumberStartHeight,
                -0.1f);
            Vector3 end = start + new Vector3(
                Random.Range(-_damageNumberHorizontalSpread, _damageNumberHorizontalSpread),
                -_damageNumberEndDrop,
                0f);

            damageObject.transform.position = start;
            damageObject.transform.localScale = Vector3.one * 0.45f;

            float duration = Mathf.Max(0.05f, _damageNumberDuration);
            Sequence sequence = DOTween.Sequence().SetUpdate(true);
            sequence.Append(damageObject.transform
                .DOScale(Vector3.one, _damageNumberPopDuration)
                .SetEase(Ease.OutBack));

            Tween arcTween = DOVirtual.Float(0f, 1f, duration, progress =>
            {
                float arc = 4f * _damageNumberArcHeight * progress * (1f - progress);
                damageObject.transform.position = Vector3.Lerp(start, end, progress)
                    + Vector3.up * arc;
            }).SetEase(Ease.Linear).SetUpdate(true);

            sequence.Append(arcTween);
            sequence.Join(damageObject.transform
                .DOScale(Vector3.zero, duration)
                .SetEase(Ease.InQuad));
            sequence.OnComplete(() => Destroy(damageObject));
        }

        private void RestartFlash(float duration)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            _coroutine = StartCoroutine(FlashCoroutine(Mathf.Max(0f, duration)));
        }

        private void ApplySquash()
        {
            if (_useSquash == false)
                return;

            _squashTween?.Kill();
            transform.localScale = _originalScale;

            Vector3 squashed = new Vector3(
                _originalScale.x * (1f + _squashAmount),
                _originalScale.y * (1f - _squashAmount),
                _originalScale.z);

            _squashTween = DOTween.Sequence()
                .Append(transform.DOScale(squashed, _squashDuration * 0.35f).SetEase(Ease.OutQuad))
                .Append(transform.DOScale(_originalScale, _squashDuration * 0.65f).SetEase(Ease.OutBack));
        }

        private static string FormatDamage(float damage)
        {
            return Mathf.Approximately(damage, Mathf.Round(damage))
                ? Mathf.RoundToInt(damage).ToString()
                : damage.ToString("0.##");
        }

        private void ApplyHitImpulse(Vector2 hitFromWorldPos)
        {
            if (Time.time < _nextImpulseTime)
                return;

            _nextImpulseTime = Time.time + _impulseCooldown;

            _impulseTween?.Kill();

            Vector2 pos = transform.position;
            Vector2 dir = (pos - hitFromWorldPos);

            if (dir.sqrMagnitude < 0.0001f)
                dir = Vector2.right;

            float dist = dir.magnitude;
            dir.Normalize();

            float knockbackMul = 1f;
            if (_minDistanceForFullKnockback > 0.0001f)
            {
                knockbackMul = Mathf.Lerp(_minKnockbackMultiplier, 1f, dist / _minDistanceForFullKnockback);
                knockbackMul = Mathf.Clamp01(knockbackMul);
                knockbackMul = Mathf.Max(knockbackMul, _minKnockbackMultiplier);
            }

            float x = dir.x * _knockbackDistance * knockbackMul;
            float y = _jumpHeight;

            if (_disableHorizontalKnockbackWhenVeryClose && dist <= _veryCloseDistance)
                x = 0f;

            Vector3 target = transform.position + new Vector3(x, y, 0f);

            _impulseTween = transform
                .DOMove(target, _impulseDuration)
                .SetEase(_impulseEase)
                .SetUpdate(false);
        }

        private IEnumerator FlashCoroutine(float duration)
        {
            _flashStrength = 1f;
            UpdateTint();

            float holdTime = duration * _flashHoldPart;
            yield return new WaitForSeconds(holdTime);

            float fadeDuration = Mathf.Max(0.0001f, duration - holdTime);
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                float t = elapsed / fadeDuration;
                _flashStrength = 1f - t * t;
                UpdateTint();
                elapsed += Time.deltaTime;
                yield return null;
            }

            _flashStrength = 0f;
            UpdateTint();
            _coroutine = null;
        }
    }
}
