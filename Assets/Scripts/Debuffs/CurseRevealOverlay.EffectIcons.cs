using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Debuffs
{
    public partial class CurseRevealOverlay
    {
        [SerializeField, Min(1f)] private float _effectIconSize = 28f;
        [SerializeField, Min(0f)] private float _effectIconSpacing = 5f;
        [SerializeField] private Vector2 _effectIconsOffset = new(76f, 0f);

        private readonly List<GameObject> _effectIconGroups = new();

        private void PrepareEffectIcons(IReadOnlyList<DebuffRoll> debuffs, float negativeEffectMultiplier)
        {
            ClearEffectIconGroups();

            if (_statsView == null || debuffs == null)
                return;

            for (int i = 0; i < debuffs.Count; i++)
            {
                if (debuffs[i] == null)
                    continue;

                Dictionary<PerkType, float> modifiersByType = new();
                foreach (DebuffModifier modifier in debuffs[i].GetModifiers(negativeEffectMultiplier))
                {
                    if (!modifiersByType.ContainsKey(modifier.Type))
                        modifiersByType.Add(modifier.Type, 1f);

                    modifiersByType[modifier.Type] *= modifier.Multiplier;
                }

                List<DebuffModifier> modifiers = new();
                foreach (KeyValuePair<PerkType, float> modifier in modifiersByType)
                {
                    Sprite statIcon = _statsView.GetStatIcon(modifier.Key);
                    if (statIcon != null && !Mathf.Approximately(modifier.Value, 1f))
                        modifiers.Add(new DebuffModifier(modifier.Key, modifier.Value));
                }

                if (i >= _icons.Count || _icons[i] == null || modifiers.Count == 0)
                {
                    _effectIconGroups.Add(null);
                    continue;
                }

                GameObject group = new GameObject("CurseEffectStats", typeof(RectTransform));
                group.transform.SetParent(_icons[i].transform.parent, false);
                RectTransform groupTransform = (RectTransform)group.transform;
                groupTransform.anchorMin = new Vector2(0.5f, 0.5f);
                groupTransform.anchorMax = new Vector2(0.5f, 0.5f);
                groupTransform.pivot = new Vector2(0f, 0.5f);
                groupTransform.anchoredPosition = _icons[i].rectTransform.anchoredPosition + _effectIconsOffset;
                groupTransform.sizeDelta = new Vector2(
                    modifiers.Count * _effectIconSize + (modifiers.Count - 1) * _effectIconSpacing,
                    _effectIconSize);

                for (int modifierIndex = 0; modifierIndex < modifiers.Count; modifierIndex++)
                    CreateEffectIcon(groupTransform, modifiers[modifierIndex], modifierIndex);

                group.SetActive(false);
                _effectIconGroups.Add(group);
            }
        }

        private void CreateEffectIcon(RectTransform parent, DebuffModifier modifier, int index)
        {
            GameObject iconObject = new GameObject("StatEffectIcon", typeof(RectTransform), typeof(Image));
            iconObject.transform.SetParent(parent, false);

            RectTransform iconTransform = (RectTransform)iconObject.transform;
            iconTransform.anchorMin = new Vector2(0f, 0.5f);
            iconTransform.anchorMax = new Vector2(0f, 0.5f);
            iconTransform.pivot = new Vector2(0.5f, 0.5f);
            iconTransform.anchoredPosition = new Vector2(
                _effectIconSize * 0.5f + index * (_effectIconSize + _effectIconSpacing), 0f);
            iconTransform.sizeDelta = new Vector2(_effectIconSize, _effectIconSize);

            Image image = iconObject.GetComponent<Image>();
            image.sprite = _statsView.GetStatIcon(modifier.Type);
            image.preserveAspect = true;
            image.raycastTarget = false;

            GameObject directionObject = new GameObject("Direction", typeof(RectTransform), typeof(TextMeshProUGUI));
            directionObject.transform.SetParent(iconTransform, false);
            RectTransform directionTransform = (RectTransform)directionObject.transform;
            directionTransform.anchorMin = new Vector2(1f, 0f);
            directionTransform.anchorMax = new Vector2(1f, 0f);
            directionTransform.pivot = new Vector2(1f, 0f);
            directionTransform.anchoredPosition = new Vector2(4f, -2f);
            directionTransform.sizeDelta = new Vector2(18f, 18f);

            TMP_Text direction = directionObject.GetComponent<TMP_Text>();
            direction.text = IsPositiveEffect(modifier) ? "↑" : "↓";
            direction.fontSize = 18f;
            direction.fontStyle = FontStyles.Bold;
            direction.alignment = TextAlignmentOptions.Center;
            direction.color = IsPositiveEffect(modifier)
                ? new Color(0.48f, 1f, 0.64f)
                : new Color(1f, 0.3f, 0.32f);
            direction.raycastTarget = false;
        }

        private void RevealEffectIconsUpTo(int visibleCharacters)
        {
            for (int i = 0; i < _effectIconGroups.Count && i < _revealAt.Count; i++)
            {
                if (_effectIconGroups[i] != null && visibleCharacters >= _revealAt[i])
                    _effectIconGroups[i].SetActive(true);
            }
        }

        private void RevealAllEffectIcons()
        {
            foreach (GameObject group in _effectIconGroups)
            {
                if (group != null)
                    group.SetActive(true);
            }
        }

        private void ClearEffectIconGroups()
        {
            foreach (GameObject group in _effectIconGroups)
            {
                if (group != null)
                    Destroy(group);
            }

            _effectIconGroups.Clear();
        }

        private static bool IsPositiveEffect(DebuffModifier modifier)
        {
            return modifier.Type == PerkType.AttackCooldown
                ? modifier.Multiplier < 1f
                : modifier.Multiplier > 1f;
        }
    }
}
