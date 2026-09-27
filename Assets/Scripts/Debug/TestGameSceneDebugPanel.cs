using System.Collections.Generic;
using System.Linq;
using CharacterLogic;
using CharacterLogic.Initializer;
using Items.BaseClass;
using Items.Enums;
using Items.ItemHolder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Debugging
{
    /// <summary>
    /// Debug overlay for TestGameScene: one button per item from ItemsHolder.
    /// First press gives the item to the player, next presses raise its level.
    /// </summary>
    public class TestGameSceneDebugPanel : MonoBehaviour
    {
        private const float PanelWidth = 200f;
        private const float ButtonHeight = 28f;
        private const float IconSize = 22f;

        private readonly List<ItemButton> _itemButtons = new();

        private CharacterInitializer _characterInitializer;
        private Character _character;

        private void Awake()
        {
            _characterInitializer = FindFirstObjectByType<CharacterInitializer>();

            if (_characterInitializer != null)
                _characterInitializer.CharacterCreated += OnCharacterCreated;
        }

        private void Start()
        {
            ItemsHolder itemsHolder = FindFirstObjectByType<ItemsHolder>();

            if (itemsHolder == null)
            {
                Debug.LogError($"{nameof(TestGameSceneDebugPanel)}: no ItemsHolder in scene, panel not built");
                return;
            }

            BuildUi(itemsHolder);
            RefreshButtons();
        }

        private void OnDestroy()
        {
            if (_characterInitializer != null)
                _characterInitializer.CharacterCreated -= OnCharacterCreated;

            SetCharacter(null);
        }

        private void OnCharacterCreated(Character character)
        {
            SetCharacter(character);
        }

        private void SetCharacter(Character character)
        {
            if (_character != null)
            {
                _character.NewItemAdded -= RefreshButtons;
                _character.ItemSwapped -= RefreshButtons;
            }

            _character = character;

            if (_character != null)
            {
                _character.NewItemAdded += RefreshButtons;
                _character.ItemSwapped += RefreshButtons;
            }

            RefreshButtons();
        }

        private void BuildUi(ItemsHolder itemsHolder)
        {
            GameObject canvasObject = new GameObject("DebugItemsCanvas",
                typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 500;

            RectTransform panel = CreatePanel(canvas.transform);

            foreach (ItemVariations variation in System.Enum.GetValues(typeof(ItemVariations)))
            {
                Item item = itemsHolder.GetItemByType(variation);

                if (item != null)
                    CreateItemButton(panel, item, variation);
            }
        }

        private RectTransform CreatePanel(Transform parent)
        {
            GameObject panelObject = new GameObject("DebugItemsPanel", typeof(Image),
                typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));

            RectTransform panel = panelObject.GetComponent<RectTransform>();
            panel.SetParent(parent, false);
            panel.anchorMin = new Vector2(0f, 0f);
            panel.anchorMax = new Vector2(0f, 0f);
            panel.pivot = new Vector2(0f, 0f);
            panel.anchoredPosition = new Vector2(10f, 10f);
            panel.sizeDelta = new Vector2(PanelWidth, 0f);

            Image background = panelObject.GetComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.55f);

            VerticalLayoutGroup layout = panelObject.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(6, 6, 6, 6);
            layout.spacing = 3f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            ContentSizeFitter fitter = panelObject.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            CreateHeader(panel);
            return panel;
        }

        private void CreateHeader(RectTransform panel)
        {
            GameObject headerObject = new GameObject("Header", typeof(TextMeshProUGUI));
            headerObject.transform.SetParent(panel, false);

            TextMeshProUGUI header = headerObject.GetComponent<TextMeshProUGUI>();
            header.text = "DEBUG ITEMS";
            header.fontSize = 14f;
            header.fontStyle = FontStyles.Bold;
            header.color = Color.yellow;
            header.alignment = TextAlignmentOptions.Center;

            LayoutElement layoutElement = headerObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = ButtonHeight;
            layoutElement.preferredHeight = ButtonHeight;
            layoutElement.flexibleHeight = 0f;
        }

        private void CreateItemButton(RectTransform panel, Item item, ItemVariations variation)
        {
            GameObject buttonObject = new GameObject(variation.ToString(), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(panel, false);

            Image buttonImage = buttonObject.GetComponent<Image>();
            buttonImage.color = new Color(1f, 1f, 1f, 0.18f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = buttonImage;
            button.interactable = true;

            LayoutElement buttonLayout = buttonObject.AddComponent<LayoutElement>();
            buttonLayout.minHeight = ButtonHeight;
            buttonLayout.preferredHeight = ButtonHeight;
            buttonLayout.flexibleHeight = 0f;

            HorizontalLayoutGroup row = buttonObject.AddComponent<HorizontalLayoutGroup>();
            row.padding = new RectOffset(4, 4, 0, 0);
            row.spacing = 6f;
            row.childControlWidth = true;
            row.childControlHeight = true;
            row.childForceExpandWidth = false;
            row.childForceExpandHeight = false;

            CreateIcon(row.transform, item);
            TextMeshProUGUI label = CreateLabel(row.transform);

            button.onClick.AddListener(() => OnItemButtonPressed(variation));

            _itemButtons.Add(new ItemButton(variation, item, button, label));
        }

        private void CreateIcon(Transform parent, Item item)
        {
            GameObject iconObject = new GameObject("Icon", typeof(Image));
            iconObject.transform.SetParent(parent, false);

            Image icon = iconObject.GetComponent<Image>();
            icon.sprite = item.VisualData.Sprite;
            icon.color = Color.white;
            icon.preserveAspect = true;

            if (icon.sprite == null)
                iconObject.SetActive(false);

            LayoutElement layoutElement = iconObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = IconSize;
            layoutElement.minHeight = IconSize;
            layoutElement.preferredWidth = IconSize;
            layoutElement.preferredHeight = IconSize;
            layoutElement.flexibleWidth = 0f;
            layoutElement.flexibleHeight = 0f;
        }

        private TextMeshProUGUI CreateLabel(Transform parent)
        {
            GameObject labelObject = new GameObject("Label", typeof(TextMeshProUGUI));
            labelObject.transform.SetParent(parent, false);

            TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
            label.fontSize = 13f;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.overflowMode = TextOverflowModes.Ellipsis;

            LayoutElement layoutElement = labelObject.AddComponent<LayoutElement>();
            layoutElement.flexibleWidth = 1f;

            return label;
        }

        private void OnItemButtonPressed(ItemVariations variation)
        {
            if (_character == null)
                return;

            _character.SelectWheelItem(variation);
            RefreshButtons();
        }

        private void RefreshButtons()
        {
            foreach (ItemButton itemButton in _itemButtons)
            {
                Item ownedItem = _character?.Inventory.Items
                    .FirstOrDefault(item => item.Data.ItemVariation == itemButton.Variation);

                if (ownedItem == null)
                {
                    itemButton.Label.text = itemButton.Source.VisualData.Name;
                    itemButton.Button.interactable = true;
                }
                else if (ownedItem.IsMaxLevelReached())
                {
                    itemButton.Label.text = $"{itemButton.Source.VisualData.Name} MAX";
                    itemButton.Button.interactable = false;
                }
                else
                {
                    itemButton.Label.text =
                        $"{itemButton.Source.VisualData.Name} {ownedItem.CurrentLevel}/{ownedItem.Data.MaxLevel}";
                    itemButton.Button.interactable = true;
                }
            }
        }

        private class ItemButton
        {
            public ItemButton(ItemVariations variation, Item source, Button button, TextMeshProUGUI label)
            {
                Variation = variation;
                Source = source;
                Button = button;
                Label = label;
            }

            public ItemVariations Variation { get; }
            public Item Source { get; }
            public Button Button { get; }
            public TextMeshProUGUI Label { get; }
        }
    }
}
