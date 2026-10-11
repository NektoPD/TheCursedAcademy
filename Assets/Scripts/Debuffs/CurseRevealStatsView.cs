using System.Collections.Generic;
using System.Globalization;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Debuffs
{
    public class CurseRevealStatsView : MonoBehaviour
    {
        [SerializeField] private RectTransform _statsRoot;
        [SerializeField] private TMP_Text _fontSource;
        [SerializeField] private TMP_Text _startLabel;
        [SerializeField] private List<PerkVisualData> _visuals = new();

        private static readonly PerkType[] StatTypes =
        {
            PerkType.Power, PerkType.Armor, PerkType.MaxHp,
            PerkType.HpRegeneration, PerkType.AttackCooldown, PerkType.Speed,
            PerkType.Greed, PerkType.Growth, PerkType.Magnet,
            PerkType.Area, PerkType.Duration
        };

        private static readonly Color Negative = new(1f, 0.3f, 0.32f);
        private static readonly Color Positive = new(0.48f, 1f, 0.64f);
        private static readonly Color Neutral = new(0.93f, 0.94f, 1f);
        private readonly List<StatRow> _rows = new();
        private TMP_Text _legend;

        private sealed class StatRow
        {
            public RectTransform Root;
            public Image Icon;
            public TMP_Text Name;
            public TMP_Text Values;
            public readonly List<ContributionView> Contributions = new();
        }

        private sealed class ContributionView
        {
            public RectTransform Root;
            public Image Icon;
            public TMP_Text Text;
        }

        private struct Contribution
        {
            public Sprite Icon;
            public float Multiplier;
        }

        public void Show(IReadOnlyDictionary<PerkType, float> baseStats,
            IReadOnlyList<DebuffRoll> debuffs, float negativeEffectMultiplier, float coinMultiplier)
        {
            EnsureRows();

            if (_startLabel != null)
                _startLabel.text = Translator.Translate("Начать", "Start", "Başla");

            for (int i = 0; i < StatTypes.Length; i++)
            {
                PerkType type = StatTypes[i];
                var contributions = CollectContributions(type, debuffs, negativeEffectMultiplier);
                float multiplier = 1f;
                bool hasNegative = false;

                foreach (Contribution contribution in contributions)
                {
                    multiplier *= contribution.Multiplier;
                    hasNegative |= IsNegative(type, contribution.Multiplier);
                }

                if (type == PerkType.Greed && coinMultiplier != 1f)
                {
                    multiplier *= coinMultiplier;
                    contributions.Add(new Contribution
                    {
                        Icon = FindVisual(type)?.Sprite,
                        Multiplier = coinMultiplier
                    });
                    hasNegative |= IsNegative(type, coinMultiplier);
                }

                float baseline = 0f;
                if (baseStats != null)
                    baseStats.TryGetValue(type, out baseline);

                UpdateRow(_rows[i], type, baseline, multiplier, hasNegative, contributions);
            }

            _legend.text = Translator.Translate(
                "Характеристики: до -> после\n% — множитель, даже при нуле\nКрасный — каждый штраф проклятия",
                "Stats: before -> after\n% = multiplier, even at zero\nRed = each curse penalty",
                "Özellikler: önce -> sonra\n% = çarpan, sıfırda bile\nKırmızı = her lanet cezası");
        }

        private static List<Contribution> CollectContributions(PerkType type,
            IReadOnlyList<DebuffRoll> debuffs, float negativeEffectMultiplier)
        {
            var contributions = new List<Contribution>();
            if (debuffs == null)
                return contributions;

            foreach (DebuffRoll roll in debuffs)
            {
                if (roll == null)
                    continue;

                // Keep each modifier explicit: a positive modifier must not hide a penalty.
                foreach (DebuffModifier modifier in roll.GetModifiers(negativeEffectMultiplier))
                {
                    if (modifier.Type == type && modifier.Multiplier != 1f)
                    {
                        contributions.Add(new Contribution
                        {
                            Icon = roll.Icon,
                            Multiplier = modifier.Multiplier
                        });
                    }
                }
            }

            return contributions;
        }

        private void EnsureRows()
        {
            // Show can run while the overlay is inactive, before Awake/OnEnable.
            if (_rows.Count != 0)
                return;

            for (int i = 0; i < StatTypes.Length; i++)
            {
                RectTransform card = CreateCard(i, StatTypes[i].ToString());
                var row = new StatRow
                {
                    Root = card,
                    Icon = CreateIcon(card, "StatIcon"),
                    Name = CreateText(card, "Name", 24f),
                    Values = CreateText(card, "Values", 22f)
                };
                SetRect(row.Icon.rectTransform, new Vector2(0f, 0.52f), new Vector2(0f, 1f),
                    new Vector2(10f, 7f), new Vector2(54f, -9f));
                SetRect(row.Name.rectTransform, new Vector2(0f, 0.57f), Vector2.one,
                    new Vector2(64f, 0f), new Vector2(-10f, -5f));
                SetRect(row.Values.rectTransform, new Vector2(0f, 0.29f), new Vector2(1f, 0.62f),
                    new Vector2(64f, 0f), new Vector2(-10f, 0f));
                _rows.Add(row);
            }

            _legend = CreateText(CreateCard(11, "Legend"), "Legend", 20f);
            SetRect(_legend.rectTransform, Vector2.zero, Vector2.one,
                new Vector2(14f, 8f), new Vector2(-14f, -8f));
            _legend.color = Neutral;
        }

        private RectTransform CreateCard(int index, string name)
        {
            RectTransform card = CreateRect(name, _statsRoot);
            int column = index % 3;
            int row = index / 3;
            SetRect(card, new Vector2(column / 3f, 1f - (row + 1) / 4f),
                new Vector2((column + 1) / 3f, 1f - row / 4f),
                new Vector2(4f, 4f), new Vector2(-4f, -4f));
            Image background = card.gameObject.AddComponent<Image>();
            background.color = new Color(0.035f, 0.045f, 0.08f, 0.9f);
            background.raycastTarget = false;
            return card;
        }

        private void UpdateRow(StatRow row, PerkType type, float baseline, float multiplier,
            bool hasNegative, List<Contribution> contributions)
        {
            PerkVisualData visual = FindVisual(type);
            row.Name.text = visual != null ? visual.Name.Trim() : type.ToString();
            row.Icon.sprite = visual != null ? visual.Sprite : null;
            row.Icon.enabled = row.Icon.sprite != null;
            row.Icon.color = hasNegative ? Negative : Neutral;
            row.Values.text = $"{Number(baseline)} -> {Number(baseline * multiplier)}  ({Percent(multiplier)})";
            row.Values.color = EffectColor(type, multiplier);

            while (row.Contributions.Count < contributions.Count)
            {
                RectTransform root = CreateRect("CurseContribution", row.Root);
                Image icon = CreateIcon(root, "CurseIcon");
                TMP_Text text = CreateText(root, "Percent", 19f);
                SetRect(icon.rectTransform, Vector2.zero, new Vector2(0f, 1f),
                    new Vector2(0f, 2f), new Vector2(24f, -2f));
                SetRect(text.rectTransform, Vector2.zero, Vector2.one,
                    new Vector2(28f, 0f), Vector2.zero);
                row.Contributions.Add(new ContributionView { Root = root, Icon = icon, Text = text });
            }

            for (int i = 0; i < row.Contributions.Count; i++)
            {
                ContributionView view = row.Contributions[i];
                view.Root.gameObject.SetActive(i < contributions.Count);
                if (i >= contributions.Count)
                    continue;

                Contribution contribution = contributions[i];
                SetRect(view.Root, new Vector2((float)i / contributions.Count, 0f),
                    new Vector2((float)(i + 1) / contributions.Count, 0.28f),
                    new Vector2(10f, 5f), new Vector2(-4f, 0f));
                view.Icon.sprite = contribution.Icon;
                view.Icon.enabled = contribution.Icon != null;
                view.Icon.color = EffectColor(type, contribution.Multiplier);
                view.Text.text = Percent(contribution.Multiplier);
                view.Text.color = view.Icon.color;
            }
        }

        private PerkVisualData FindVisual(PerkType type)
        {
            foreach (PerkVisualData visual in _visuals)
            {
                if (visual != null && visual.Type == type)
                    return visual;
            }

            return null;
        }

        private TMP_Text CreateText(RectTransform parent, string name, float size)
        {
            TMP_Text text = CreateRect(name, parent).gameObject.AddComponent<TextMeshProUGUI>();
            text.font = _fontSource != null && _fontSource.font != null
                ? _fontSource.font : TMP_Settings.defaultFontAsset;
            text.fontSize = size;
            text.enableAutoSizing = true;
            text.fontSizeMin = 14f;
            text.fontSizeMax = size;
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.enableWordWrapping = false;
            text.raycastTarget = false;
            text.color = Neutral;
            return text;
        }

        private static Image CreateIcon(RectTransform parent, string name)
        {
            Image image = CreateRect(name, parent).gameObject.AddComponent<Image>();
            image.preserveAspect = true;
            image.raycastTarget = false;
            return image;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            var child = new GameObject(name, typeof(RectTransform));
            child.layer = parent.gameObject.layer;
            var rect = (RectTransform)child.transform;
            rect.SetParent(parent, false);
            return rect;
        }

        private static void SetRect(RectTransform rect, Vector2 min, Vector2 max,
            Vector2 insetMin, Vector2 insetMax)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = insetMin;
            rect.offsetMax = insetMax;
        }

        private static bool IsNegative(PerkType type, float multiplier)
        {
            return type == PerkType.AttackCooldown ? multiplier > 1f : multiplier < 1f;
        }

        private static Color EffectColor(PerkType type, float multiplier)
        {
            if (multiplier == 1f)
                return Neutral;

            return IsNegative(type, multiplier) ? Negative : Positive;
        }

        private static string Number(float value)
        {
            return value.ToString("0.######", CultureInfo.InvariantCulture);
        }

        private static string Percent(float multiplier)
        {
            return ((multiplier - 1f) * 100f).ToString("+0.######;-0.######;0", CultureInfo.InvariantCulture) + "%";
        }
    }
}
