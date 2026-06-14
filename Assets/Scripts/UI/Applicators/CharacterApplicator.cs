using System.Collections.Generic;
using System.Linq;
using CharacterLogic.Initializer;
using Data;
using PlayerPerksController;
using TMPro;
using UI.Animation;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using WalletSystem;
using YG;
using YG.Utils;
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
        [SerializeField] private Button _playerSelectButton;
        [SerializeField] private Image _playerSelectButtonImage;
        [SerializeField] private Sprite _buySprite;
        [SerializeField] private Sprite _playSprite;
        [SerializeField] private int _gameIdScene;
        [SerializeField] private SceneChanger _changer;
        [SerializeField] private TextMeshProUGUI _attackPower;
        [SerializeField] private TextMeshProUGUI _armor;
        [SerializeField] private TextMeshProUGUI _hp;
        [SerializeField] private TextMeshProUGUI _hpRegen;
        [SerializeField] private TextMeshProUGUI _attackCooldown;
        [SerializeField] private TextMeshProUGUI _speed;
        [SerializeField] private CharacterPurchaseController _characterPurchaseController;
        [SerializeField] private WindowAnimation _error;

        private PerkController _perkController;
        private Wallet _wallet;

        [Inject]
        public void Construct(PerkController perkController, Wallet wallet)
        {
            _perkController = perkController;
            _wallet = wallet;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _playerSelectButton.onClick.AddListener(OnCharacterSelectButtonClick);
            _playerSelectButton.interactable = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _playerSelectButton.onClick.RemoveListener(OnCharacterSelectButtonClick);
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

            _playerSelectButtonImage.sprite = IsCharacterAvailable() ? _playSprite : _buySprite;
        }

        private float GetM(Dictionary<PerkType, float> m, PerkType t)
            => m != null && m.TryGetValue(t, out var v) ? v : 1f;

        private void OnCharacterSelectButtonClick()
        {
            if (!IsCharacterAvailable())
            {
                TryBuyCharacter();
                return;
            }

            _playerSelectButton.interactable = false;

            PlayerPrefs.SetInt(Key, (int)CurrentItem.Data.Type);
            _changer.ChangeScene(_gameIdScene);
        }

        private void TryBuyCharacter()
        {
            if (CurrentItem.Data.UnlockPrice > _wallet.Money)
            {
                _error.Open();
                return;
            }

            if (!_characterPurchaseController.TryUnlockCharacter(CurrentItem.Data.Type)) return;
            _wallet.RemoveMoney(CurrentItem.Data.UnlockPrice);
            Applicate(CurrentItem);
        }

        private bool IsCharacterAvailable()
        {
            return _characterPurchaseController.IsCharacterAvailable(CurrentItem.Data.Type);
        }
    }
}