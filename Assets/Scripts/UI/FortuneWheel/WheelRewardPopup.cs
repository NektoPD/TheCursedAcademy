using System;
using System.Text;
using Data;
using Items.BaseClass;
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
        [SerializeField] private AudioClip _showClip;
        [SerializeField] private AudioSource _audioSource;

        public event Action Confirmed;

        private void OnEnable() => _closeButton.onClick.AddListener(OnClosePressed);

        private void OnDisable() => _closeButton.onClick.RemoveListener(OnClosePressed);

        public void ShowItem(ItemVisualData item)
        {
            ShowItem(item, null);
        }

        public void ShowItem(ItemVisualData item, Item existingItem)
        {
            if (item == null)
                return;

            OpenUnscaledTime();
            PlayShowSound();

            SetIcon(item.Sprite);
            _title.text = item.Name;

            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(item.Description))
                builder.AppendLine(item.Description);

            var stats = existingItem != null ? existingItem.UiStats : item.Stats;

            if (stats != null)
            {
                foreach (var stat in stats)
                {
                    if (stat == null)
                        continue;

                    string value = existingItem != null && !existingItem.IsMaxLevelReached()
                        ? stat.CurrentValue.ToString("0.##") + " -> " + stat.NextValue.ToString("0.##")
                        : stat.CurrentValue.ToString("0.##");
                    builder.AppendLine(stat.Name + ": " + value);
                }
            }

            _description.text = builder.ToString();
        }

        public void ShowGold(int amount)
        {
            OpenUnscaledTime();
            PlayShowSound();

            SetIcon(_goldIcon);
            _title.text = Translator.Translate("Золото", "Gold", "Altın");
            _description.text = "+" + amount;
        }

        public void ShowBuff(WheelBuffData buff)
        {
            if (buff == null)
                return;

            OpenUnscaledTime();
            PlayShowSound();

            SetIcon(buff.Icon);
            _title.text = buff.Name;
            _description.text = Translator.Translate(
                "x" + buff.Multiplier.ToString("0.##") + " на " + buff.DurationSeconds.ToString("0.##") + " сек",
                "x" + buff.Multiplier.ToString("0.##") + " for " + buff.DurationSeconds.ToString("0.##") + " sec",
                "x" + buff.Multiplier.ToString("0.##") + " " + buff.DurationSeconds.ToString("0.##") + " sn");
        }

        private void SetIcon(Sprite sprite)
        {
            if (_icon == null)
                return;

            _icon.sprite = sprite;
            _icon.enabled = sprite != null;
        }

        private void PlayShowSound()
        {
            if (_showClip != null)
                GetAudioSource().PlayOneShot(_showClip);
        }

        private AudioSource GetAudioSource()
        {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();

            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
                _audioSource.spatialBlend = 0f;
            }

            return _audioSource;
        }

        private void OnClosePressed() => Confirmed?.Invoke();
    }
}
