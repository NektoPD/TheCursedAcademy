using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyDamageView : MonoBehaviour
{
    private readonly Color DamageColor = new (1f, 0.5f, 0.5f);

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Coroutine _coroutine;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _originalColor = _spriteRenderer.color;
    }

    public void StartFlash(float duration)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(FlashCoroutine(duration));
    }

    private IEnumerator FlashCoroutine(float duration)
    {
        float halfDuration = duration / 2f;
        Color flashColor = DamageColor;

        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            float t = elapsed / halfDuration; 
            _spriteRenderer.color = Color.Lerp(_originalColor, flashColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            float t = elapsed / halfDuration; 
            _spriteRenderer.color = Color.Lerp(flashColor, _originalColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _spriteRenderer.color = _originalColor;
    }
}
