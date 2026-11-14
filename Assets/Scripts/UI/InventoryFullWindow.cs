using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using InventorySystem;
using Items.BaseClass;
using Items.ItemHolder;
using TheraBytes.BetterUi;
using TMPro;
using UI.Applicators;
using UI.Applicators.ClickHandlers;
using UI.FortuneWheel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace UI
{
    public class InventoryFullWindow : Window, IScrollHandler
    {
        [SerializeField] private List<ItemView> _itemsVisual;
        [SerializeField] private ItemApplicatorCurrentOnly _applicator;
        [SerializeField] private BetterScrollRect _betterScrollRect;

        [Header("Offered item")]
        [SerializeField] private WheelRewardView _offeredItemView;
        [SerializeField] private ItemClickHandler _offeredItemClickHandler;
        [SerializeField] private TMP_Text _selectionHint;
        [SerializeField] private GameObject _selectedItemDetails;

        [Header("Decision")]
        [SerializeField] private Button _swapButton;
        [SerializeField] private Button _declineButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private TMP_Text _declineButtonText;
        [SerializeField, Min(0)] private int _declineGoldAmount = 30;
        [SerializeField] private GameObject _modalBackdrop;

        [SerializeField, Range(0.05f, 0.5f)] private float _scrollStep = 0.2f;

        private ItemsHolder _itemsHolder;
        private CharacterInventory _inventory;
        private int _countItems;
        private bool _decisionPending;
        private bool _isClosing;
        private bool _hasSelectableItems;

        public event Action<int> Declined;

        [Inject]
        private void Construct(ItemsHolder holder)
        {
            _itemsHolder = holder;
        }

        private void Start()
        {
            _betterScrollRect.horizontalNormalizedPosition = 0f;
        }

        private void OnEnable()
        {
            _offeredItemClickHandler.Clicked += PreviewOfferedItem;
            _modalBackdrop.transform.SetAsLastSibling();
            transform.SetAsLastSibling();
            _modalBackdrop.SetActive(true);
        }

        private void OnDisable()
        {
            _offeredItemClickHandler.Clicked -= PreviewOfferedItem;
            _modalBackdrop.SetActive(false);
            _decisionPending = false;
            SetDecisionButtonsInteractable(false);
        }

        private void ScrollLeft()
        {
            float newPos = _betterScrollRect.horizontalNormalizedPosition - _scrollStep;
            _betterScrollRect.horizontalNormalizedPosition = Mathf.Clamp01(newPos);
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (_betterScrollRect != null && _betterScrollRect.isActiveAndEnabled)
                _betterScrollRect.OnScroll(eventData);
        }

        private void ScrollRight()
        {
            float newPos = _betterScrollRect.horizontalNormalizedPosition + _scrollStep;
            _betterScrollRect.horizontalNormalizedPosition = Mathf.Clamp01(newPos);
        }

        public void Initialize(CharacterInventory inventory)
        {
            _inventory = inventory;

            _countItems = _inventory.InventoryLimit;

            for (var i = 0; i < _itemsVisual.Count; i++)
            {
                bool gameObjectStatus = i + 1 <= _countItems;
                _itemsVisual[i].gameObject.SetActive(gameObjectStatus);
            }
        }

        public void Show(ItemVisualData offeredItem)
        {
            if (offeredItem == null)
                throw new ArgumentNullException(nameof(offeredItem));

            if (_isClosing && gameObject.activeSelf)
                return;

            _offeredItemClickHandler.SetData(offeredItem);
            _offeredItemView.Initialize(offeredItem.Name, offeredItem.Description, offeredItem.Sprite);
            OpenUnscaledTime();
        }

        private void PreviewOfferedItem(ItemVisualData offeredItem)
        {
            if (!_decisionPending || _isClosing || offeredItem == null)
                return;

            Item item = _itemsHolder.GetItemByType(offeredItem.Variation);
            if (item == null)
                return;

            _selectedItemDetails.SetActive(true);
            _applicator.PreviewItem(item);
        }

        public override void OpenWindow() => OpenUnscaledTime();

        public override void OpenUnscaledTime()
        {
            if (_isClosing && gameObject.activeSelf)
                return;

            _decisionPending = true;
            _isClosing = false;
            List<Item> itemsWithoutStart = PrepareInventory();
            RefreshDecisionText();
            SetDecisionButtonsInteractable(true);
            base.OpenUnscaledTime();

            RefreshItemViews(itemsWithoutStart);
            _applicator.enabled = _hasSelectableItems;
        }

        public void CompleteSwap()
        {
            if (!TryBeginClosing())
                return;

            base.CloseUnscaledTime();
        }

        public override void CloseWindow() => CloseUnscaledTime();

        public override void CloseUnscaledTime()
        {
            if (!TryBeginClosing())
                return;

            Declined?.Invoke(Mathf.Max(0, _declineGoldAmount));
            base.CloseUnscaledTime();
        }

        private bool TryBeginClosing()
        {
            if (!_decisionPending || _isClosing || !gameObject.activeInHierarchy)
                return false;

            _decisionPending = false;
            _isClosing = true;
            SetDecisionButtonsInteractable(false);

            _applicator.enabled = false;
            return true;
        }

        private void SetDecisionButtonsInteractable(bool interactable)
        {
            _offeredItemClickHandler.enabled = interactable;
            _swapButton.interactable = interactable && _hasSelectableItems;
            _declineButton.interactable = interactable;
            _closeButton.interactable = interactable;
        }

        private void RefreshDecisionText()
        {
            _selectionHint.text = _hasSelectableItems
                ? Translator.Translate(
                    "Выбери предмет для замены", "Choose an item to replace", "Değiştirilecek eşyayı seç")
                : Translator.Translate(
                    "Нет предметов для замены. Можно отказаться и получить золото.",
                    "No items can be replaced. Decline to receive gold.",
                    "Değiştirilebilecek eşya yok. Altın almak için vazgeç.");

            int goldAmount = Mathf.Max(0, _declineGoldAmount);
            _declineButtonText.text = Translator.Translate(
                $"Отказаться (+{goldAmount} <sprite=\"coin\" index=0>)",
                $"Decline (+{goldAmount} <sprite=\"coin\" index=0>)",
                $"Vazgeç (+{goldAmount} <sprite=\"coin\" index=0>)");
        }

        private List<Item> PrepareInventory()
        {
            _applicator.enabled = false;

            List<Item> itemsWithoutStart = _inventory.Items
                .Where(item => item != null && item.VisualData != null
                    && item.VisualData.Variation != _inventory.StartVariation)
                .ToList();

            List<ItemVisualData> visualDatasInInventory = itemsWithoutStart
                .Select(item => item.VisualData)
                .ToList();

            _applicator.Initialize(itemsWithoutStart);
            _applicator.Inizialize(visualDatasInInventory);
            _applicator.SetDefaultItem(visualDatasInInventory.FirstOrDefault());
            _hasSelectableItems = itemsWithoutStart.Count > 0;
            _selectedItemDetails.SetActive(_hasSelectableItems);
            return itemsWithoutStart;
        }

        private void RefreshItemViews(IReadOnlyList<Item> itemsWithoutStart)
        {
            for (int i = 0; i < _itemsVisual.Count; i++)
            {
                if (i < itemsWithoutStart.Count && itemsWithoutStart[i] != null)
                {
                    _itemsVisual[i].gameObject.SetActive(true);
                    _itemsVisual[i].Initialize(itemsWithoutStart[i].VisualData, false,
                        itemsWithoutStart[i].CurrentLevel, itemsWithoutStart[i].IsMaxLevelReached());

                    continue;
                }

                _itemsVisual[i].gameObject.SetActive(false);
            }
        }
    }
}