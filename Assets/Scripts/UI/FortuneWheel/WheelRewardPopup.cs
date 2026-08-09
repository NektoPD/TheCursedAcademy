using System;
using System.Text;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.FortuneWheel
{
    public class WheelRewardPopup : UI.Window
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Sprite _goldIcon;

        public event Action Confirmed;

        private void OnEnable() => _closeButton.onClick.AddListener(OnClosePressed);

        private void OnDisable() => _closeButton.onClick.RemoveListener(OnClosePressed);

        public void ShowItem(ItemVisualData item)
        {
            if (item == null)
                return;

            SetIcon(item.Sprite);
            _title.text = item.Name;

            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(item.Description))
                builder.AppendLine(item.Description);

            if (item.Stats != null)
            {
                foreach (var stat in item.Stats)
                {
                    if (stat == null)
                        continue;

                    builder.AppendLine(stat.Name + ": " + stat.CurrentValue.ToString("0.##"));
                }
            }

            _description.text = builder.ToString();
            OpenUnscaledTime();
        }

        public void ShowGold(int amount)
        {
            SetIcon(_goldIcon);
            _title.text = Translator.Translate("Золото", "Gold", "Altın");
            _description.text = "+" + amount;
            OpenUnscaledTime();
        }

        public void ShowBuff(WheelBuffData buff)
        {
            if (buff == null)
                return;

            SetIcon(buff.Icon);
            _title.text = buff.Name;
            _description.text = Translator.Translate(
                "x" + buff.Multiplier.ToString("0.##") + " на " + buff.DurationSeconds.ToString("0.##") + " сек",
                "x" + buff.Multiplier.ToString("0.##") + " for " + buff.DurationSeconds.ToString("0.##") + " sec",
                "x" + buff.Multiplier.ToString("0.##") + " " + buff.DurationSeconds.ToString("0.##") + " sn");
            OpenUnscaledTime();
        }

        private void SetIcon(Sprite sprite)
        {
            if (_icon == null)
                return;

            _icon.sprite = sprite;
            _icon.enabled = sprite != null;
        }

        private void OnClosePressed() => Confirmed?.Invoke();
    }
}
