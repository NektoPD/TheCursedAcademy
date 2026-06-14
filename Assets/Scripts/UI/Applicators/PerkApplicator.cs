using Data;
using PlayerPerksController;
using System;
using System.Linq;
using TMPro;
using UI.Applicators.ClickHandlers;
using UnityEngine;
using UnityEngine.UI;
using WalletSystem;
using Zenject;

namespace UI.Applicators
{
    public class PerkApplicator : BaseApplicator<PerkVisualData>
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Image _image;
        [SerializeField] private Image _item;
        [SerializeField] private TextMeshProUGUI _cost;
        [SerializeField] private Button _buy;
        [SerializeField] private GameObject _error;

        private PerkController _perkController;
        private Wallet _wallet;

        public event Action<PerkVisualData> Buyed;

        public PerkController PerkController => _perkController;

        [Inject]
        public void Construct(PerkController perkController, Wallet wallet)
        {
            _perkController = perkController;
            _wallet = wallet;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _buy.onClick.AddListener(OnBuyClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _buy.onClick.RemoveListener(OnBuyClick);
        }

        protected override void Applicate(PerkVisualData data)
        {
            UpdatePerkText(data);
        }

        private void UpdatePerkText(PerkVisualData data)
        {
            _name.text = data.Name;

            if (_perkController.GetPerkLevel(data.Type) < _perkController.MaxUpgradeCount)
            {
                _description.text =
                    $"{data.Description} ({_perkController.GetPerkLevel(data.Type) * 5}% => {(_perkController.GetPerkLevel(data.Type) + 1) * 5}%)";
            }
            else
            {
                _description.text = $"{data.Description} ({_perkController.GetPerkLevel(data.Type) * 5}%)";
            }

            _image.sprite = data.Sprite;
            _cost.text = (data.DefaultPrice * (_perkController.GetPerkLevel(data.Type) + 1)).ToString();
        }

        private void OnBuyClick()
        {
            int perkPrice = CurrentItem.DefaultPrice * (_perkController.GetPerkLevel(CurrentItem.Type) + 1);

            if (perkPrice > _wallet.Money)
            {
                _error.SetActive(true);
                return;
            }

            if (!_perkController.TryUpgradePerk(CurrentItem.Type))
                return;

            _wallet.RemoveMoney(perkPrice);
            Buyed?.Invoke(CurrentItem);
            UpdatePerkText(CurrentItem);
        }
    }
}