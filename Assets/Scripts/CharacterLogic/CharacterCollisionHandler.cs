using System;
using ExpPoints;
using UnityEngine;
using WalletSystem.MoneyLogic;
using WalletSystem;
using Zenject;

namespace CharacterLogic
{
    public class CharacterCollisionHandler : MonoBehaviour
    {
        [Inject] private readonly Wallet _wallet;

        public event Action<int> GotMoney;
        public event Action<int> GotExpPoint;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Money money))
            {
                GotMoney?.Invoke(money.Value);
                //_wallet.AddMoney(money.Value); тут я думаю, что лучше сохранить куда-то кол-во собранных денег, а потом в конце при смерти сразу записывать их в кошель, чтобы нельзя было выйти из меню паузы и деньги оставались на счёту
                money.Despawn();
            }

            if (collision.gameObject.TryGetComponent(out IExpPoint expPoint))
            {
                GotExpPoint?.Invoke(expPoint.Value);
                //тут добавление в лвл кол-во поинтов expPoint.Value
                expPoint.Despawn();
            }
        }
    }
}