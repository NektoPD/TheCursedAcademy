using System;
using System.Collections.Generic;
using UnityEngine;
using PickableItems;
using Utils;

namespace CharacterLogic
{
    public class CharacterCollisionHandler : MonoBehaviour
    {
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _maxRiseHeight = 1f;
        [SerializeField] private float _maxPushDistance = 2f;

        private class AttractedItem
        {
            public Transform Transform;
            public Vector2 StartPosition;
            public float Timer;
        }

        private readonly Dictionary<IPickable, AttractedItem> _items = new();
        private readonly List<IPickable> _iterationBuffer = new();

        public event Action<int> GotMoney;
        public event Action<int> GotExpPoint;
        public event Action<int> GotHeal;

        private void Update()
        {
            if (_items.Count == 0)
                return;

            _iterationBuffer.Clear();
            _iterationBuffer.AddRange(_items.Keys);

            float deltaTime = Time.deltaTime;
            Vector2 targetPosition = transform.position;

            foreach (var pickable in _iterationBuffer)
            {
                if (!_items.TryGetValue(pickable, out var data) || data.Transform == null)
                {
                    _items.Remove(pickable);
                    continue;
                }

                data.Timer += deltaTime;
                float progress = data.Timer / _duration;

                if (progress >= 1f)
                {
                    CallEvent(pickable);
                    pickable.Despawn();
                    _items.Remove(pickable);
                    continue;
                }

                MoveItemAlongArc(data.Transform, data.StartPosition, targetPosition, progress);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out IPickable pickable))
                return;

            if (_items.ContainsKey(pickable))
                return;

            Transform tr = collision.transform;

            _items[pickable] = new AttractedItem
            {
                Transform = tr,
                StartPosition = tr.position,
                Timer = 0f
            };
        }

        private void MoveItemAlongArc(
            Transform item,
            Vector2 startPos,
            Vector2 targetPos,
            float progress)
        {
            float pushProgress = Mathf.Clamp01(progress * 2f);
            Vector2 pushOffset = (startPos - targetPos).normalized * _maxPushDistance;
            Vector2 pushPoint = startPos + pushOffset;

            float riseProgress = Mathf.Clamp01((progress - 0.5f) * 2f);
            Vector2 risePoint = pushPoint + Vector2.up * _maxRiseHeight;

            Vector2 finalPos = Vector2.Lerp(
                Vector2.Lerp(startPos, pushPoint, pushProgress),
                Vector2.Lerp(risePoint, targetPos, riseProgress),
                progress
            );

            item.position = finalPos;
        }

        private void CallEvent(IPickable item)
        {
            switch (item)
            {
                case Money money:
                    GotMoney?.Invoke(money.Value);
                    break;

                case ExpPoint expPoint:
                    GotExpPoint?.Invoke(expPoint.Value);
                    break;

                case Heal heal:
                    GotHeal?.Invoke(heal.Value);
                    break;

                case Magnet magnet:
                    foreach (var kv in magnet.GetAllActivePickableItems())
                    {
                        if (_items.ContainsKey(kv.Key))
                            continue;

                        _items[kv.Key] = new AttractedItem
                        {
                            Transform = kv.Value,
                            StartPosition = kv.Value.position,
                            Timer = 0f
                        };
                    }
                    break;
            }
        }
    }
}
