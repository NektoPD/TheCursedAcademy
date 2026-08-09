using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using DG.Tweening;
using Items.ItemHolder;
using InventorySystem;
using UnityEngine;
using Zenject;

namespace UI.FortuneWheel
{
    public class FortuneWheelWindow : UI.Window
    {
        private const int SlotsCount = 8;

        [Header("Wheel")]
        [SerializeField] private RectTransform _wheel;
        [SerializeField] private RectTransform _pointer;
        [SerializeField] private List<WheelSlot> _slots = new();

        [Header("Reward Pool Weights")]
        [SerializeField, Range(0, 8)] private int _itemSlots = 4;
        [SerializeField, Range(0, 8)] private int _goldSlots = 2;
        [SerializeField] private List<int> _goldAmounts = new() { 25, 50, 75, 100 };
        [SerializeField] private List<WheelBuffData> _buffLibrary = new();

        [Header("Spin")]
        [SerializeField] private float _appearDuration = 0.4f;
        [SerializeField] private Ease _appearEase = Ease.OutBack;
        [SerializeField] private float _openDelay = 0.6f;
        [SerializeField] private int _fullSpins = 5;
        [SerializeField] private float _spinDuration = 3f;
        [SerializeField] private Ease _spinEase = Ease.InOutCubic;
        [SerializeField] private float _holdDelayBeforeClose = 1.5f;

        private readonly List<WheelReward> _rewards = new();
        private ItemsHolder _itemsHolder;
        private CharacterInventory _inventory;
        private Coroutine _routine;

        public event Action<ItemVisualData> ItemRewarded;
        public event Action<int> GoldRewarded;
        public event Action<WheelBuffData> BuffRewarded;
        public event Action Finished;

        [Inject]
        private void Construct(ItemsHolder holder)
        {
            _itemsHolder = holder;
        }

        public void Initialize(CharacterInventory inventory) => _inventory = inventory;

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

        private void Play()
        {
            if (_routine != null)
                StopCoroutine(_routine);

            _routine = StartCoroutine(PlayRoutine());
        }

        private IEnumerator PlayRoutine()
        {
            BuildRewards();

            for (int i = 0; i < _slots.Count; i++)
                _slots[i].StopPulse();

            for (int i = 0; i < _slots.Count && i < _rewards.Count; i++)
                _slots[i].Set(_rewards[i]);

            if (_wheel != null)
            {
                _wheel.localRotation = Quaternion.identity;
                yield return AppearWheel();
            }

            yield return new WaitForSecondsRealtime(_openDelay);

            yield return SpinRandom();

            int winningIndex = DetectWinningSlotIndex();

            if (winningIndex >= 0 && winningIndex < _slots.Count)
                _slots[winningIndex].PlayPulse();

            yield return new WaitForSecondsRealtime(_holdDelayBeforeClose);

            if (winningIndex >= 0 && winningIndex < _rewards.Count)
                ApplyReward(_rewards[winningIndex]);

            Finished?.Invoke();
            _routine = null;
        }

        private IEnumerator AppearWheel()
        {
            _wheel.localScale = Vector3.zero;

            bool done = false;

            _wheel.DOScale(Vector3.one, _appearDuration)
                .SetEase(_appearEase)
                .SetUpdate(true)
                .OnComplete(() => done = true);

            while (!done)
                yield return null;
        }

        private IEnumerator SpinRandom()
        {
            if (_wheel == null)
                yield break;

            float randomOffset = UnityEngine.Random.Range(0f, 360f);
            float finalAngle = _fullSpins * 360f + randomOffset;

            bool done = false;

            _wheel.DORotate(new Vector3(0f, 0f, -finalAngle), _spinDuration, RotateMode.FastBeyond360)
                .SetEase(_spinEase)
                .SetUpdate(true)
                .OnComplete(() => done = true);

            while (!done)
                yield return null;
        }

        private int DetectWinningSlotIndex()
        {
            if (_slots == null || _slots.Count == 0)
                return -1;

            Vector2 center = _wheel != null ? (Vector2)_wheel.position : Vector2.zero;
            Vector2 pointerDir = _pointer != null
                ? ((Vector2)_pointer.position - center).normalized
                : Vector2.up;

            int bestIndex = -1;
            float bestAngle = float.MaxValue;

            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i] == null)
                    continue;

                Vector2 slotDir = ((Vector2)_slots[i].transform.position - center).normalized;
                float angle = Vector2.Angle(pointerDir, slotDir);

                if (angle < bestAngle)
                {
                    bestAngle = angle;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private void BuildRewards()
        {
            _rewards.Clear();

            int itemCount = Mathf.Clamp(_itemSlots, 0, SlotsCount);
            int goldCount = Mathf.Clamp(_goldSlots, 0, SlotsCount - itemCount);
            int buffCount = SlotsCount - itemCount - goldCount;

            _itemsHolder.GetVisualDatas(itemCount, out List<ItemVisualData> itemDatas);

            for (int i = 0; i < itemCount; i++)
            {
                ItemVisualData data = i < itemDatas.Count ? itemDatas[i] : _itemsHolder.GetVisualData();
                _rewards.Add(WheelReward.CreateItem(data));
            }

            for (int i = 0; i < goldCount; i++)
                _rewards.Add(WheelReward.CreateGold(PickGold()));

            for (int i = 0; i < buffCount; i++)
                _rewards.Add(WheelReward.CreateBuff(PickBuff()));

            Shuffle(_rewards);
        }

        private int PickGold()
        {
            if (_goldAmounts == null || _goldAmounts.Count == 0)
                return 50;

            return _goldAmounts[UnityEngine.Random.Range(0, _goldAmounts.Count)];
        }

        private WheelBuffData PickBuff()
        {
            if (_buffLibrary == null || _buffLibrary.Count == 0)
                return null;

            return _buffLibrary[UnityEngine.Random.Range(0, _buffLibrary.Count)];
        }

        private void ApplyReward(WheelReward reward)
        {
            switch (reward.Type)
            {
                case WheelRewardType.Item:
                    ItemRewarded?.Invoke(reward.Item);
                    break;
                case WheelRewardType.Gold:
                    GoldRewarded?.Invoke(reward.GoldAmount);
                    break;
                case WheelRewardType.Buff:
                    BuffRewarded?.Invoke(reward.Buff);
                    break;
            }
        }

        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
