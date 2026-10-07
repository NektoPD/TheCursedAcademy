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
    /// Debug overlay for TestGameScene: one button per item from ItemsHolder,
    /// a player level-up button and a hide/show toggle for the whole panel.
    /// First press gives the item to the player, next presses raise its level.
    /// </summary>
    public class TestGameSceneDebugPanel : MonoBehaviour
    {
        private const float PanelWidth = 200f;
        private const float ButtonHeight = 28f;
        private const float IconSize = 22f;
        private const float ToggleButtonWidth = 64f;
        private const float Margin = 10f;

        private readonly List<ItemButton> _itemButtons = new();

        private CharacterInitializer _characterInitializer;
        private Character _character;
        private RectTransform _panel;
        private TextMeshProUGUI _toggleLabel;

        private void Awake()
        {
            _characterInitializer = FindFirstObjectByType<CharacterInitializer>();

            if (_characterInitializer != null)
                _characterInitializer.CharacterCreated += OnCharacterCreated;
        }

        private void Start()
        {
            if (_characterInitializer != null && _characterInitializer.PlayerTransform != null)
                SetCharacter(_characterInitializer.PlayerTransform.GetComponent<Character>());

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
            canvasObject.transform.SetParent(transform, false);

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 500;

            _panel = CreatePanel(canvas.transform);
            CreateToggleButton(canvas.transform);

            CreateActionButton(_panel, "Level Up", OnLevelUpPressed);

            foreach (ItemVariations variation in System.Enum.GetValues(typeof(ItemVariations)))
            {
                Item item = itemsHolder.GetItemByType(variation);

                if (item != null)
                    CreateItemButton(_panel, item, variation);
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
            panel.anchoredPosition = new Vector2(Margin, Margin + ButtonHeight + Margin);
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

        private void CreateToggleButton(Transform parent)
        {
            GameObject buttonObject = new GameObject("DebugToggleButton", typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(Margin, Margin);
            rect.sizeDelta = new Vector2(ToggleButtonWidth, ButtonHeight);

            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.55f);

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(OnToggleButtonPressed);

            GameObject labelObject = new GameObject("Label", typeof(TextMeshProUGUI));
            labelObject.transform.SetParent(buttonObject.transform, false);

            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            _toggleLabel = labelObject.GetComponent<TextMeshProUGUI>();
            _toggleLabel.fontSize = 12f;
            _toggleLabel.fontStyle = FontStyles.Bold;
            _toggleLabel.color = Color.yellow;
            _toggleLabel.alignment = TextAlignmentOptions.Center;
            _toggleLabel.text = "Hide";
        }

        private void OnToggleButtonPressed()
        {
            if (_panel == null)
                return;

            bool visible = !_panel.gameObject.activeSelf;
            _panel.gameObject.SetActive(visible);
            _toggleLabel.text = visible ? "Hide" : "Show";
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

        private Button CreateRowButton(RectTransform panel, string name, out TextMeshProUGUI label)
        {
            GameObject buttonObject = new GameObject(name, typeof(Image), typeof(Button));
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

            label = CreateLabel(row.transform);
            return button;
        }

        private void CreateActionButton(RectTransform panel, string title, System.Action onPressed)
        {
            Button button = CreateRowButton(panel, title, out TextMeshProUGUI label);

            label.fontStyle = FontStyles.Bold;
            label.color = new Color(1f, 0.85f, 0.3f);
            label.alignment = TextAlignmentOptions.Center;
            label.text = title;

            button.onClick.AddListener(() => onPressed());
        }

        private void OnLevelUpPressed()
        {
            _character?.LevelUpImmediately();
        }

        private void CreateItemButton(RectTransform panel, Item item, ItemVariations variation)
        {
            Button button = CreateRowButton(panel, variation.ToString(), out TextMeshProUGUI label);

            CreateIcon(button.transform, item).transform.SetSiblingIndex(0);

            button.onClick.AddListener(() => OnItemButtonPressed(variation));

            _itemButtons.Add(new ItemButton(variation, item, button, label));
        }

        private Image CreateIcon(Transform parent, Item item)
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

            return icon;
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
                    itemButton.Button.interactable = _character != null;
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
