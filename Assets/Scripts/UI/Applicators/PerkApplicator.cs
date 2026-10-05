using Data;
using DG.Tweening;
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
        [SerializeField] private AudioSource _upgradeSound;

        private PerkController _perkController;
        private Wallet _wallet;
        private Tween _selectionTween;
        private Tween _purchaseTween;
        private Tween _descriptionTween;
        private Vector3 _imageScale;
        private Vector3 _buyScale;

        public event Action<PerkVisualData> Buyed;
        public event Action<PerkVisualData> Selected;

        public PerkController PerkController => _perkController;

        private void Awake()
        {
            _imageScale = _image.transform.localScale;
            _buyScale = _buy.transform.localScale;
        }

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
            _selectionTween?.Kill();
            _purchaseTween?.Kill();
            _descriptionTween?.Kill();
            _description.alpha = 1f;
            _image.transform.localScale = _imageScale;
            _buy.transform.localScale = _buyScale;
        }

        protected override void OnItemSelected(PerkVisualData data)
        {
            Selected?.Invoke(data);
            _selectionTween?.Kill();
            _image.transform.localScale = _imageScale;
            _selectionTween = _image.transform.DOScale(_imageScale * 1.12f, 0.14f)
                .SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo).SetUpdate(true);
            _descriptionTween?.Kill();
            _description.alpha = 0f;
            _descriptionTween = _description.DOFade(1f, 0.3f).SetUpdate(true);
        }

        protected override void Applicate(PerkVisualData data)
        {
            UpdatePerkText(data);
        }

        private void UpdatePerkText(PerkVisualData data)
        {
            _name.text = data.Name;

            int stepPercent = Mathf.RoundToInt(_perkController.GetUpgradeStep(data.Type) * 100f);

            if (_perkController.GetPerkLevel(data.Type) < _perkController.MaxUpgradeCount)
            {
                int currentPercent = stepPercent * _perkController.GetPerkLevel(data.Type);
                _description.text = $"{data.Description} ({currentPercent}% => {currentPercent + stepPercent}%)";
            }
            else
            {
                _description.text = $"{data.Description} ({stepPercent * _perkController.GetPerkLevel(data.Type)}%)";
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
            _purchaseTween?.Kill();
            _buy.transform.localScale = _buyScale;
            _purchaseTween = _buy.transform.DOScale(_buyScale * 1.18f, 0.18f)
                .SetEase(Ease.OutBack).SetLoops(2, LoopType.Yoyo).SetUpdate(true);
            if (_upgradeSound != null && _upgradeSound.clip != null)
                _upgradeSound.PlayOneShot(_upgradeSound.clip);

            Buyed?.Invoke(CurrentItem);
            UpdatePerkText(CurrentItem);
        }
    }
}