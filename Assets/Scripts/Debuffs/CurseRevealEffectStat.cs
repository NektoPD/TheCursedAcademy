using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Debuffs
{
    public sealed class CurseRevealEffectStat : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _percent;
        private bool _prepared;

        public void Prepare(Sprite icon, float multiplier, PerkType type)
        {
            _prepared = true;

            if (_icon != null)
            {
                _icon.sprite = icon;
                _icon.enabled = icon != null;
                _icon.raycastTarget = false;
            }

            if (_percent != null)
            {
                _percent.text = FormatPercent(multiplier);
                _percent.color = IsPositiveEffect(type, multiplier)
                    ? new Color(0.48f, 1f, 0.64f)
                    : new Color(1f, 0.3f, 0.32f);
                _percent.raycastTarget = false;
            }

            gameObject.SetActive(false);
        }

        public void Reveal()
        {
            if (_prepared)
                gameObject.SetActive(true);
        }

        public void Hide()
        {
            _prepared = false;
            gameObject.SetActive(false);
        }

        private static bool IsPositiveEffect(PerkType type, float multiplier)
        {
            return type == PerkType.AttackCooldown
                ? multiplier < 1f
                : multiplier > 1f;
        }

        private static string FormatPercent(float multiplier)
        {
            return ((multiplier - 1f) * 100f)
                .ToString("+0.##;-0.##;0", CultureInfo.InvariantCulture) + "%";
        }
    }
}
