using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Debuffs
{
    public class SlotColumn : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private float _spinInterval = 0.06f;

        private IReadOnlyList<DebuffData> _pool;
        private Coroutine _spinRoutine;
        private DebuffData _result;

        public DebuffData Result => _result;

        public void Initialize(IReadOnlyList<DebuffData> pool)
        {
            _pool = pool;
        }

        public void StartSpin()
        {
            if (_spinRoutine != null)
                StopCoroutine(_spinRoutine);

            _spinRoutine = StartCoroutine(SpinRoutine());
        }

        public void StopOn(DebuffData result)
        {
            if (_spinRoutine != null)
            {
                StopCoroutine(_spinRoutine);
                _spinRoutine = null;
            }

            _result = result;
            _icon.sprite = result.Icon;
        }

        private IEnumerator SpinRoutine()
        {
            var wait = new WaitForSecondsRealtime(_spinInterval);

            while (true)
            {
                if (_pool != null && _pool.Count > 0)
                    _icon.sprite = _pool[Random.Range(0, _pool.Count)].Icon;

                yield return wait;
            }
        }
    }
}
