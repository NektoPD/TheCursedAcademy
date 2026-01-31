using System.Collections.Generic;
using System.Linq;
using Data;
using PlayerPerksController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using WalletSystem;
using Zenject;

namespace UI.Applicators
{
    public class CharacterApplicator : BaseApplicator<CharacterVisualData>
    {
        private const string Key = "CharacterId";
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Image _image;
        [SerializeField] private Image _item;
        [SerializeField] private Button _play;
        [SerializeField] private int _gameIdScene;
        [SerializeField] private SceneChanger _changer;
        [SerializeField] private TextMeshProUGUI _attackPower;
        [SerializeField] private TextMeshProUGUI _armor;
        [SerializeField] private TextMeshProUGUI _hp;
        [SerializeField] private TextMeshProUGUI _hpRegen;
        [SerializeField] private TextMeshProUGUI _attackCooldown;
        [SerializeField] private TextMeshProUGUI _speed;

        private PerkController _perkController;

        [Inject]
        public void Construct(PerkController perkController)
        {
            _perkController = perkController;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _play.onClick.AddListener(OnPlayClick);
            _play.interactable = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _play.onClick.RemoveListener(OnPlayClick);
        }

        protected override void Applicate(CharacterVisualData data)
        {
            _name.text = data.Name;
            _description.text = $"{data.Description}\n{data.StartItem}";
            _image.sprite = data.Sprite;
            _item.sprite = data.Data.StartItem.Data.ItemIcon;

            Dictionary<PerkType, float> m = _perkController.GetFinalPerkValues();

            _attackPower.text = (data.Data.AttackPower * GetM(m, PerkType.Power)).ToString("0.##");
            _armor.text = (data.Data.Armor * GetM(m, PerkType.Armor)).ToString("0.##");
            _hp.text = (data.Data.Hp * GetM(m, PerkType.MaxHp)).ToString("0.##");
            _hpRegen.text = (data.Data.HpRegenerationSpeed * GetM(m, PerkType.HpRegeneration)).ToString("0.##");
            _attackCooldown.text =
                (data.Data.AttackRegenerationSpeed * GetM(m, PerkType.AttackCooldown)).ToString("0.##");
            _speed.text = (data.Data.MoveSpeed * GetM(m, PerkType.Speed)).ToString("0.##");
        }

        private float GetM(Dictionary<PerkType, float> m, PerkType t)
            => m != null && m.TryGetValue(t, out var v) ? v : 1f;

        private void OnPlayClick()
        {
            _play.interactable = false;
            
            PlayerPrefs.SetInt(Key, (int)CurrentItem.Data.Type);
            _changer.ChangeScene(_gameIdScene);
        }
    }
}