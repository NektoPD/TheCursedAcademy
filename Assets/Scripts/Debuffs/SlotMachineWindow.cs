using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Debuffs
{
    public class SlotMachineWindow : UI.Window
    {
        private const int ColumnsCount = 3;

        [SerializeField] private List<DebuffData> _debuffLibrary = new();
        [SerializeField] private List<SlotColumn> _columns = new();
        [SerializeField] private List<TMP_Text> _resultTexts = new();
        [SerializeField] private float _openDelay = 0.5f;
        [SerializeField] private float _spinDuration = 1.2f;
        [SerializeField] private float _delayBetweenStops = 0.6f;
        [SerializeField] private float _holdDelayBeforeClose = 1.5f;
        [SerializeField] private float _pulseScale = 1.15f;
        [SerializeField] private float _pulseDuration = 0.5f;

        private readonly List<DebuffRoll> _selected = new();
        private Coroutine _routine;

        public event Action<IReadOnlyList<DebuffRoll>> Finished;

        public override void OpenWindow()
        {
            base.OpenWindow();
            Play();
        }

        public override void OpenUnscaledTime()
        {
            base.OpenUnscaledTime();
            Play();
        }

        public void Play()
        {
            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(PlayRoutine());
        }

        private IEnumerator PlayRoutine()
        {
            _selected.Clear();
            _selected.AddRange(PickDistinct(ColumnsCount));

            yield return new WaitForSecondsRealtime(_openDelay);

            for (int i = 0; i < _columns.Count; i++)
            {
                _columns[i].Initialize(_debuffLibrary);
                _columns[i].StartSpin();
            }

            yield return new WaitForSecondsRealtime(_spinDuration);

            for (int i = 0; i < _columns.Count; i++)
            {
                _columns[i].Stop(_selected[i]);

                while (!_columns[i].IsStopped)
                    yield return null;

                if (i < _resultTexts.Count && _resultTexts[i] != null)
                {
                    _resultTexts[i].text = _selected[i].Name;
                    PulseText(_resultTexts[i]);
                }

                yield return new WaitForSecondsRealtime(_delayBetweenStops);
            }

            yield return new WaitForSecondsRealtime(_holdDelayBeforeClose);

            Finished?.Invoke(_selected);
            _routine = null;
        }

        private void PulseText(TMP_Text text)
        {
            text.rectTransform.localScale = Vector3.one;
            text.rectTransform
                .DOScale(_pulseScale, _pulseDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true);
        }

        private IEnumerable<DebuffRoll> PickDistinct(int count)
        {
            return _debuffLibrary
                .OrderBy(_ => UnityEngine.Random.value)
                .Take(Mathf.Min(count, _debuffLibrary.Count))
                .Select(data => new DebuffRoll(data, UnityEngine.Random.Range(0, data.VariantCount)));
        }
    }
}
