using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using InventorySystem;
using Items.BaseClass;
using Items.ItemHolder;
using TheraBytes.BetterUi;
using UI.Applicators;
using UnityEngine;
using Zenject;

namespace UI
{
    public class InventoryFullWindow : Window
    {
        [SerializeField] private List<ItemView> _itemsVisual;
        [SerializeField] private ItemApplicatorCurrentOnly _applicator;
        [SerializeField] private BetterScrollRect _betterScrollRect;

        [SerializeField] private UnityEngine.UI.Button _leftButton;
        [SerializeField] private UnityEngine.UI.Button _rightButton;

        [SerializeField, Range(0.05f, 0.5f)] private float _scrollStep = 0.2f;

        private ItemsHolder _itemsHolder;
        private CharacterInventory _inventory;
        private int _countItems;

        [Inject]
        private void Construct(ItemsHolder holder)
        {
            _itemsHolder = holder;
        }

        private void Start()
        {
            _betterScrollRect.horizontalNormalizedPosition = 0f;

            _leftButton.onClick.AddListener(ScrollLeft);
            _rightButton.onClick.AddListener(ScrollRight);
        }

        private void Update()
        {
            _leftButton.interactable = _betterScrollRect.horizontalNormalizedPosition > 0f;
            _rightButton.interactable = _betterScrollRect.horizontalNormalizedPosition < 1f;
        }

        private void ScrollLeft()
        {
            float newPos = _betterScrollRect.horizontalNormalizedPosition - _scrollStep;
            _betterScrollRect.horizontalNormalizedPosition = Mathf.Clamp01(newPos);
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

        public override void OpenUnscaledTime()
        {
            List<Item> itemsWithoutStart = _inventory.Items
                .Where(item => item.VisualData.Variation != _inventory.StartVariation)
                .ToList();

            List<ItemVisualData> visualDatasInInventory = itemsWithoutStart
                .Select(item => item.VisualData)
                .ToList();

            if (visualDatasInInventory.Count > 0)
            {
                _applicator.SetDefaultItem(visualDatasInInventory.First());
                _applicator.Inizialize(visualDatasInInventory);
                _applicator.Initialize(itemsWithoutStart);
            }

            base.OpenWindow();

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