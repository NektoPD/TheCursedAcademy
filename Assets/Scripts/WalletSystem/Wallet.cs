using System;
using YG;

namespace WalletSystem
{
    public class Wallet 
    {
        public WalletWrapper WalletWrapper { get; private set; }

        public event Action WalletChanged;

        public int Money => WalletWrapper.Money;

        public void Initialize()
        {
            WalletWrapper = YandexGame.SDKEnabled ? YandexGame.savesData.WalletWrapper : new WalletWrapper();
        }

        public void AddMoney(int money)
        {
            if (money < 0)
                return;

            WalletWrapper.Money += money;
            YandexGame.SaveProgress();
            WalletChanged?.Invoke();
        }

        public void RemoveMoney(int money)
        {
            if (WalletWrapper.Money < money)
                return;

            WalletWrapper.Money -= money;
            YandexGame.SaveProgress();
            WalletChanged?.Invoke();
        }
    }

    [Serializable]
    public class WalletWrapper
    {
        public int Money = 0;
    }
}
