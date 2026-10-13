using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Debuffs
{
    public class CurseRevealStatView : MonoBehaviour
    {
        public enum MainStat
        {
            Power = 0,
            Armor = 1,
            MaxHp = 2,
            HpRegeneration = 3,
            AttackCooldown = 4,
            Speed = 5
        }

        [SerializeField] private MainStat _stat;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _value;
        [SerializeField] private List<EffectSlot> _effectSlots = new();

        private static readonly Color Negative = new(1f, 0.3f, 0.32f);
        private static readonly Color Positive = new(0.48f, 1f, 0.64f);
        private static readonly Color Neutral = new(0.93f, 0.94f, 1f);

        private float _baseline;
        private float _multiplier;
        private int _effectIndex;

        public PerkType Type => (PerkType)_stat;

        public void BeginPreview(float baseline, Sprite statIcon)
        {
            _baseline = baseline;
            _multiplier = 1f;
            _effectIndex = 0;

            if (_icon != null)
            {
                _icon.sprite = statIcon;
                _icon.enabled = statIcon != null;
            }

            foreach (EffectSlot slot in _effectSlots)
                slot?.Hide();

            if (_name != null)
                _name.text = GetName();
        }

        public void AddEffect(Sprite icon, float multiplier)
        {
            _multiplier *= multiplier;

            if (_effectIndex < _effectSlots.Count)
                _effectSlots[_effectIndex]?.Show(icon, multiplier, Type);

            _effectIndex++;
        }

        public void CompletePreview()
        {
            if (_value != null)
            {
                _value.text = $"{Number(_baseline)} -> {Number(_baseline * _multiplier)} ({Percent(_multiplier)})";
                _value.color = EffectColor(Type, _multiplier);
            }
        }

        private string GetName()
        {
            return _stat switch
            {
                MainStat.Power => Translator.Translate("Сила атаки", "Attack power", "Saldırı gücü"),
                MainStat.Armor => Translator.Translate("Броня", "Armor", "Zırh"),
                MainStat.MaxHp => Translator.Translate("Здоровье", "Health", "Can"),
                MainStat.HpRegeneration => Translator.Translate("Реген. HP/с", "HP regen/s", "Can yen./sn"),
                MainStat.AttackCooldown => Translator.Translate("Интервал атак (с)", "Attack interval (s)", "Atak aralığı (sn)"),
                MainStat.Speed => Translator.Translate("Скорость бега", "Move speed", "Hareket hızı"),
                _ => _stat.ToString()
            };
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
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }

        private static string Percent(float multiplier)
        {
            return ((multiplier - 1f) * 100f).ToString("+0.##;-0.##;0", CultureInfo.InvariantCulture) + "%";
        }

        [Serializable]
        private class EffectSlot
        {
            [SerializeField] private GameObject _root;
            [SerializeField] private Image _icon;
            [SerializeField] private TMP_Text _percentage;

            public void Hide()
            {
                if (_root != null)
                    _root.SetActive(false);
            }

            public void Show(Sprite sprite, float multiplier, PerkType type)
            {
                Color color = EffectColor(type, multiplier);

                if (_icon != null)
                {
                    _icon.sprite = sprite;
                    _icon.enabled = sprite != null;
                }

                if (_percentage != null)
                {
                    _percentage.text = Percent(multiplier);
                    _percentage.color = color;
                }

                if (_root != null)
                    _root.SetActive(true);
            }
        }
    }
}
