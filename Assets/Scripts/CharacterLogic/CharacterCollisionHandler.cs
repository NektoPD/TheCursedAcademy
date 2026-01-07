using System;
using UnityEngine;
using System.Collections.Generic;
using Utils;
using PickableItems;
using Unity.VisualScripting;

namespace CharacterLogic
{
    public class CharacterCollisionHandler : MonoBehaviour
    {
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _maxRiseHeight = 1f;
        [SerializeField] private float _maxPushDistance = 2f;

        private Dictionary<IPickable, Transform> _itemsToAttract = new();
        private Dictionary<IPickable, float> _itemTimers = new Dictionary<IPickable, float>();
        private Dictionary<IPickable, Vector2> _startPositions = new Dictionary<IPickable, Vector2>();

        public event Action<int> GotMoney;
        public event Action<int> GotExpPoint;
        public event Action<int> GotHeal;

        private void Update()
        {
            var items = new List<IPickable>(_itemsToAttract.Keys);

            foreach (var pickable in items)
            {
                if (!_itemsToAttract.TryGetValue(pickable, out Transform item) || item == null)
                {
                    _itemsToAttract.Remove(pickable);
                    _itemTimers.Remove(pickable);
                    _startPositions.Remove(pickable);
                    continue;
                }

                if (!_itemTimers.ContainsKey(pickable))
                    _itemTimers[pickable] = 0f;

                _itemTimers[pickable] += Time.deltaTime;
                float progress = _itemTimers[pickable] / _duration;

                if (progress >= 1f)
                {
                    RemoveItem(pickable);
                    CallEvent(pickable);
                    pickable.Despawn(); 
                    continue;
                }

                MoveItemAlongArc(pickable, item, progress);
            }
        }

        private void AddItemToAttract(IPickable pickable, Transform tr)
        {
            if (pickable == null || tr == null) return;

            _itemsToAttract[pickable] = tr;

            if (!_startPositions.ContainsKey(pickable))
                _startPositions[pickable] = tr.position;

            if (!_itemTimers.ContainsKey(pickable))
                _itemTimers[pickable] = 0f;
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out IPickable pickable))
            {
                AddItemToAttract(pickable, collision.transform);
            }
        }

        private void MoveItemAlongArc(IPickable pickable, Transform item, float progress)
        {
            if (!_startPositions.TryGetValue(pickable, out var startPos))
            {
                // страховка: если вдруг добавили без старта Ч берЄм текущую позицию
                startPos = item.position;
                _startPositions[pickable] = startPos;
            }

            Vector2 targetPos = transform.position;

            float pushProgress = Mathf.Clamp01(progress * 2f);
            Vector2 pushOffset = (startPos - targetPos).normalized * _maxPushDistance;
            Vector2 pushPoint = startPos + pushOffset;

            float riseProgress = Mathf.Clamp01((progress - 0.5f) * 2f);
            Vector2 risePoint = pushPoint + Vector2.up * _maxRiseHeight;

            Vector2 arcPosition = Vector2.Lerp(
                Vector2.Lerp(startPos, pushPoint, pushProgress),
                Vector2.Lerp(risePoint, targetPos, riseProgress),
                progress
            );

            item.position = arcPosition;
        }

        private void CallEvent(IPickable item)
        {
            switch (item)
            {
                case Money money:
                    GotMoney?.Invoke(item.Value);
                    break;

                case ExpPoint expPoint:
                    GotExpPoint?.Invoke(item.Value);
                    break;

                case Heal heal:
                    GotHeal?.Invoke(item.Value);
                    break;

                case Magnet magnet:
                    foreach (var kv in magnet.GetAllActivePickableItems())
                        AddItemToAttract(kv.Key, kv.Value);
                    break;
            }
        }

        private void RemoveItem(IPickable pickable)
        {
            if (_itemsToAttract.ContainsKey(pickable))
            {
                _itemsToAttract.Remove(pickable);
                _itemTimers.Remove(pickable);
                _startPositions.Remove(pickable);
            }
        }
    }
}