using TMPro;
using UnityEngine;
using WalletSystem;
using Zenject;

namespace UI
{
    public class MoneyView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        private Wallet _wallet;

        private void OnEnable()
        {
            _wallet.WalletChanged += Show;
        }

        private void OnDisable()
        {
            _wallet.WalletChanged -= Show;
        }

        [Inject]
        public void Construct(Wallet wallet)
        {
            _wallet = wallet;
        }

        private void Start()
        {
            Show();
        }

        private void Show()
        {
            _text.text = _wallet.Money.ToString();
        }
    }
}