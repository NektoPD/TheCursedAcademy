using System;
using ExpPoints;
using UnityEngine;
using WalletSystem.MoneyLogic;
using System.Collections.Generic;
using Utils;

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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Money money))
            {
                //_wallet.AddMoney(money.Value); тут я думаю, что лучше сохранить куда-то кол-во собранных денег, а потом в конце при смерти сразу записывать их в кошель, чтобы нельзя было выйти из меню паузы и деньги оставались на счёту
                _itemsToAttract[money] = collision.transform;
                _startPositions[money] = collision.transform.position;
            }

            if (collision.gameObject.TryGetComponent(out ExpPoint expPoint))
            {
                //тут добавление в лвл кол-во поинтов expPoint.Value
                _itemsToAttract[expPoint] = collision.transform;
                _startPositions[expPoint] = collision.transform.position;
            }
        }

        private void MoveItemAlongArc(IPickable pickable, Transform item, float progress)
        {
            Vector2 startPos = _startPositions[pickable];
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
            if (item is Money)
                GotMoney?.Invoke(item.Value);
            else
                GotExpPoint?.Invoke(item.Value);
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