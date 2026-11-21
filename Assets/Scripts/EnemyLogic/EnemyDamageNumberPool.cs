using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace EnemyLogic
{
    public sealed class EnemyDamageNumberPool : MonoBehaviour
    {
        private const int Capacity = 32;
        private const int MaxLabelsPerFrame = 8;
        private const int WarmupBatchSize = 4;

        private static EnemyDamageNumberPool _instance;

        private readonly Stack<Label> _available = new(Capacity);
        private readonly List<Label> _active = new(Capacity);
        private TMP_FontAsset _warmupFont;
        private int _createdCount;
        private int _lastShowFrame = -1;
        private int _shownThisFrame;

        public static EnemyDamageNumberPool GetOrCreate(TMP_FontAsset font)
        {
            if (_instance != null)
                return _instance;

            var poolObject = new GameObject("Enemy Damage Numbers");
            _instance = poolObject.AddComponent<EnemyDamageNumberPool>();
            _instance._warmupFont = font;
            _instance.WarmUp(MaxLabelsPerFrame);
            return _instance;
        }

        public void Show(float damage, TMP_FontAsset font, Color color, int sortingLayerId, int sortingOrder,
            Vector3 start, float horizontalSpread, float endDrop, float arcHeight, float popDuration, float duration)
        {
            int frame = Time.frameCount;
            if (_lastShowFrame != frame)
            {
                _lastShowFrame = frame;
                _shownThisFrame = 0;
            }

            if (_shownThisFrame >= MaxLabelsPerFrame || _available.Count == 0)
                return;

            _shownThisFrame++;
            Label label = _available.Pop();
            if (label.Text.font != font)
                label.Text.font = font;

            label.Text.color = color;
            label.Text.text = FormatDamage(damage);
            label.Renderer.sortingLayerID = sortingLayerId;
            label.Renderer.sortingOrder = sortingOrder;

            start.x += Random.Range(-0.08f, 0.08f);
            label.Start = start;
            label.End = start + new Vector3(Random.Range(-horizontalSpread, horizontalSpread), -endDrop, 0f);
            label.ArcHeight = arcHeight;
            label.PopDuration = Mathf.Max(0f, popDuration);
            label.Duration = Mathf.Max(0.05f, duration);
            label.StartTime = Time.unscaledTime;
            label.Transform.position = start;
            label.Transform.localScale = Vector3.one * (label.PopDuration > 0f ? 0.45f : 1f);
            label.Renderer.enabled = true;
            _active.Add(label);
            enabled = true;
        }

        private void Update()
        {
            if (_createdCount < Capacity)
                WarmUp(WarmupBatchSize);

            float now = Time.unscaledTime;
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                Label label = _active[i];
                float elapsed = now - label.StartTime;
                if (elapsed < label.PopDuration)
                {
                    float scale = DOVirtual.EasedValue(0.45f, 1f, elapsed / label.PopDuration, Ease.OutBack);
                    label.Transform.localScale = Vector3.one * scale;
                    continue;
                }

                float progress = (elapsed - label.PopDuration) / label.Duration;
                if (progress >= 1f)
                {
                    label.Renderer.enabled = false;
                    int last = _active.Count - 1;
                    _active[i] = _active[last];
                    _active.RemoveAt(last);
                    _available.Push(label);
                    continue;
                }

                float arc = 4f * label.ArcHeight * progress * (1f - progress);
                label.Transform.position = Vector3.Lerp(label.Start, label.End, progress) + Vector3.up * arc;
                label.Transform.localScale = Vector3.one * (1f - progress * progress);
            }

            if (_createdCount == Capacity && _active.Count == 0)
                enabled = false;
        }

        private void WarmUp(int count)
        {
            int target = Mathf.Min(Capacity, _createdCount + count);
            while (_createdCount < target)
            {
                var labelObject = new GameObject("EnemyDamageNumber");
                labelObject.transform.SetParent(transform, false);
                var text = labelObject.AddComponent<TextMeshPro>();
                text.font = _warmupFont;
                text.fontSize = 5f;
                text.alignment = TextAlignmentOptions.Center;
                text.enableWordWrapping = false;
                text.overflowMode = TextOverflowModes.Overflow;

                MeshRenderer renderer = labelObject.GetComponent<MeshRenderer>();
                renderer.enabled = false;
                _available.Push(new Label(text, renderer));
                _createdCount++;
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private static string FormatDamage(float damage)
        {
            return Mathf.Approximately(damage, Mathf.Round(damage))
                ? Mathf.RoundToInt(damage).ToString()
                : damage.ToString("0.##");
        }

        private sealed class Label
        {
            public readonly TextMeshPro Text;
            public readonly MeshRenderer Renderer;
            public readonly Transform Transform;
            public Vector3 Start;
            public Vector3 End;
            public float ArcHeight;
            public float PopDuration;
            public float Duration;
            public float StartTime;

            public Label(TextMeshPro text, MeshRenderer renderer)
            {
                Text = text;
                Renderer = renderer;
                Transform = text.transform;
            }
        }
    }
}
