using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Debuffs
{
    public class SlotColumn : MonoBehaviour
    {
        [SerializeField] private RectTransform _strip;
        [SerializeField] private List<Image> _cells = new();
        [SerializeField] private float _cellHeight = 200f;
        [SerializeField] private float _spinSpeed = 1800f;
        [SerializeField] private float _settleDuration = 0.35f;

        private IReadOnlyList<DebuffData> _pool;
        private Coroutine _spinRoutine;
        private DebuffData _result;
        private bool _isStopped = true;

        public bool IsStopped => _isStopped;
        public DebuffData Result => _result;

        public void Initialize(IReadOnlyList<DebuffData> pool)
        {
            _pool = pool;
        }

        public void StartSpin()
        {
            if (_spinRoutine != null)
                StopCoroutine(_spinRoutine);

            _isStopped = false;
            _spinRoutine = StartCoroutine(SpinRoutine());
        }

        public void Stop(DebuffData result)
        {
            if (_spinRoutine != null)
                StopCoroutine(_spinRoutine);

            _result = result;
            _spinRoutine = StartCoroutine(SettleRoutine(result));
        }

        private IEnumerator SpinRoutine()
        {
            while (true)
            {
                Scroll(_spinSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
        }

        private IEnumerator SettleRoutine(DebuffData result)
        {
            float speed = _spinSpeed;
            float elapsed = 0f;

            while (elapsed < _settleDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / _settleDuration);
                float currentSpeed = Mathf.Lerp(_spinSpeed, 0f, t);
                Scroll(currentSpeed * Time.unscaledDeltaTime);
                yield return null;
            }

            AlignToCenter(result);
            _spinRoutine = null;
            _isStopped = true;
        }

        private void Scroll(float delta)
        {
            var position = _strip.anchoredPosition;
            position.y -= delta;

            while (position.y <= -_cellHeight)
            {
                position.y += _cellHeight;
                RecycleTopToBottom();
            }

            _strip.anchoredPosition = position;
        }

        private void RecycleTopToBottom()
        {
            if (_cells.Count == 0)
                return;

            Image top = _cells[0];
            _cells.RemoveAt(0);
            _cells.Add(top);

            top.rectTransform.SetAsLastSibling();
            top.sprite = GetRandomIcon();
        }

        private void AlignToCenter(DebuffData result)
        {
            _strip.anchoredPosition = new Vector2(_strip.anchoredPosition.x, 0f);

            if (_cells.Count == 0)
                return;

            int center = _cells.Count / 2;
            _cells[center].sprite = result.Icon;
        }

        private Sprite GetRandomIcon()
        {
            if (_pool == null || _pool.Count == 0)
                return null;

            return _pool[Random.Range(0, _pool.Count)].Icon;
        }
    }
}
