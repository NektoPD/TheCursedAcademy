using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private float _spinDuration = 1.2f;
        [SerializeField] private float _delayBetweenStops = 0.6f;

        private readonly List<DebuffData> _selected = new();
        private Coroutine _routine;

        public event Action<IReadOnlyList<DebuffData>> Finished;

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

            for (int i = 0; i < _columns.Count; i++)
            {
                _columns[i].Initialize(_debuffLibrary);
                _columns[i].StartSpin();
            }

            yield return new WaitForSecondsRealtime(_spinDuration);

            for (int i = 0; i < _columns.Count; i++)
            {
                _columns[i].StopOn(_selected[i]);

                if (i < _resultTexts.Count && _resultTexts[i] != null)
                    _resultTexts[i].text = _selected[i].Name;

                yield return new WaitForSecondsRealtime(_delayBetweenStops);
            }

            Finished?.Invoke(_selected);
            _routine = null;
        }

        private IEnumerable<DebuffData> PickDistinct(int count)
        {
            return _debuffLibrary
                .OrderBy(_ => UnityEngine.Random.value)
                .Take(Mathf.Min(count, _debuffLibrary.Count));
        }
    }
}
