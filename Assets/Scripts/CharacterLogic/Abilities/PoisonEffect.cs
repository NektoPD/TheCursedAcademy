using System.Collections;
using EnemyLogic;
using HealthSystem;
using UnityEngine;

namespace CharacterLogic.Abilities
{
    public class PoisonEffect : MonoBehaviour
    {
        private const int PoisonTicks = 5;

        private EnemyDamageView _enemyDamageView;
        private int _activeApplications;
        private int _generation;

        public void Apply(IDamageable target, float damage, float duration)
        {
            if (damage <= 0f || duration <= 0f || target is EnemyDamageTaker { IsDied: true })
                return;

            if (_enemyDamageView == null)
                TryGetComponent(out _enemyDamageView);

            _activeApplications++;
            _enemyDamageView?.StartPoisonPulse();
            StartCoroutine(PoisonRoutine(target, damage, duration, _generation));
        }

        private IEnumerator PoisonRoutine(IDamageable target, float damage, float duration, int generation)
        {
            float tickDamage = damage / PoisonTicks;
            float tickInterval = duration / PoisonTicks;

            for (int i = 0; i < PoisonTicks; i++)
            {
                if (target is not Component component || component == null ||
                    !component.gameObject.activeInHierarchy || target is EnemyDamageTaker { IsDied: true })
                    break;

                target.TakeDamage(tickDamage);
                if (generation != _generation)
                    yield break;

                if (target is EnemyDamageTaker { IsDied: true })
                    break;

                yield return new WaitForSeconds(tickInterval);
            }

            if (generation != _generation)
                yield break;

            _activeApplications--;
            if (_activeApplications == 0)
                _enemyDamageView?.StopPoisonPulse();
        }

        private void OnDisable()
        {
            _generation++;
            StopAllCoroutines();
            _activeApplications = 0;
            _enemyDamageView?.StopPoisonPulse();
        }
    }
}
