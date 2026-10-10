using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Debuffs
{
    public class SlotMachineWindow : UI.Window, IPointerDownHandler
    {
        private const int ColumnsCount = 3;

        [SerializeField] private List<DebuffData> _debuffLibrary = new();
        [SerializeField] private List<SlotColumn> _columns = new();
        [SerializeField] private List<TMP_Text> _resultTexts = new();
        [SerializeField] private float _openDelay = 0.5f;
        [SerializeField] private float _spinDuration = 1.2f;
        [SerializeField] private AudioSource _spinSound;
        [SerializeField] private float _delayBetweenStops = 0.6f;
        [SerializeField] private float _holdDelayBeforeClose = 1.5f;
        [SerializeField] private float _pulseScale = 1.15f;
        [SerializeField] private float _pulseDuration = 0.5f;

        private readonly List<DebuffRoll> _selected = new();
        private Coroutine _routine;
        private bool _hasOpened;
        private bool _isFinishing;

        public event Action<IReadOnlyList<DebuffRoll>> Finished;

        private void OnEnable()
        {
            _hasOpened = false;
            Opened += OnOpened;
        }

        private void OnOpened()
        {
            _hasOpened = true;
        }

        public override void OpenWindow()
        {
            _hasOpened = false;
            base.OpenWindow();
            Play();
        }

        public override void OpenUnscaledTime()
        {
            _hasOpened = false;
            base.OpenUnscaledTime();
            Play();
        }

        public void Play()
        {
            if (_routine != null)
                StopCoroutine(_routine);

            _spinSound?.Stop();
            _isFinishing = false;
            _routine = StartCoroutine(PlayRoutine());
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || _routine == null
                || !_hasOpened || _isFinishing)
                return;

            StopCoroutine(_routine);
            _routine = null;
            _isFinishing = true;
            _spinSound?.Stop();

            for (int i = 0; i < _columns.Count; i++)
            {
                _columns[i].StopImmediately(_selected[i]);
                ShowResult(i);
            }

            _routine = StartCoroutine(FinishAfterSkip());
        }

        private IEnumerator FinishAfterSkip()
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, _holdDelayBeforeClose));

            _routine = null;
            Finished?.Invoke(_selected);
        }

        private void OnDisable()
        {
            Opened -= OnOpened;
            _hasOpened = false;
            _isFinishing = false;

            if (_routine != null)
                StopCoroutine(_routine);

            _routine = null;
            _spinSound?.Stop();
        }

        private IEnumerator PlayRoutine()
        {
            _selected.Clear();
            _selected.AddRange(PickDistinct(ColumnsCount));

            yield return new WaitForSecondsRealtime(_openDelay);

            float timingScale = 1f;
            float initialSpinDuration = Mathf.Max(0f, _spinDuration);

            _spinSound.Play();

            float startedAt = Time.unscaledTime;
            float stopAt = initialSpinDuration * timingScale;

            for (int i = 0; i < _columns.Count; i++)
            {
                _columns[i].Initialize(_debuffLibrary);
                _columns[i].StartSpin();
            }

            for (int i = 0; i < _columns.Count; i++)
            {
                while (Time.unscaledTime - startedAt < stopAt)
                    yield return null;

                float settleDuration = _columns[i].SettleDuration * timingScale;
                _columns[i].Stop(_selected[i], settleDuration);

                while (!_columns[i].IsStopped)
                    yield return null;

                ShowResult(i);

                stopAt += settleDuration + Mathf.Max(0f, _delayBetweenStops) * timingScale;
            }

            _spinSound?.Stop();
            _isFinishing = true;
            yield return new WaitForSecondsRealtime(_delayBetweenStops);
            yield return new WaitForSecondsRealtime(_holdDelayBeforeClose);

            _routine = null;
            Finished?.Invoke(_selected);
        }

        private void ShowResult(int index)
        {
            if (index >= _resultTexts.Count || _resultTexts[index] == null)
                return;

            _resultTexts[index].text = _selected[index].Name;
            PulseText(_resultTexts[index]);
        }

        private void PulseText(TMP_Text text)
        {
            text.rectTransform.DOKill();
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