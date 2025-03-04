using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class HealthBar : MonoBehaviour
{
    protected Health Health;
    protected Slider Bar;

    private Coroutine _coroutine;

    private readonly float _smoothDecreaseDuration = 0.5f;

    private void OnDisable()
    {
        Health.Changed -= OnHealthChanged;
    }

    public virtual void SetHealth(Health health)
    {
        Health = health;

        Bar.maxValue = Health.MaxHealth;
        Bar.value = Health.MaxHealth;

        health.Changed += OnHealthChanged;
    }

    private void OnHealthChanged(float health)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(Countdown(health));
    }

    private IEnumerator Countdown(float health)
    {
        float elasedTime = 0f;
        float currentValue = Bar.value;

        while (elasedTime < _smoothDecreaseDuration)
        {
            elasedTime += Time.deltaTime;
            float normalizedPosition = elasedTime / _smoothDecreaseDuration;

            Bar.value = Mathf.Lerp(currentValue, health, normalizedPosition);

            yield return null;
        }
    }
}
