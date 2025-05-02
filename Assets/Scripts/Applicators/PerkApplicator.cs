using Data;
using PlayerPerksController;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WalletSystem;
using Zenject;

namespace Applicators
{
    public class PerkApplicator : BaseApplicator<PerkVisualData>
    {
        [SerializeField] private TextMeshProUGUI _cost;
        [SerializeField] private Button _buy;
        [SerializeField] private GameObject _error;

        private PerkController _perkController;
        private Wallet _wallet;

        public event Action<PerkVisualData> Buyed;

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
            Name.text = data.Name;
            Description.text = data.Description;
            MainImage.sprite = data.Sprite;
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
            
            if(_perkController.TryUpgradePerk(CurrentItem.Type) == false)
                return;

            _wallet.RemoveMoney(perkPrice);
            Buyed?.Invoke(CurrentItem);
        }
    }
}