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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Money money))
            {
                _wallet.AddMoney(money.Value);
                money.Despawn();
            }

            if (collision.gameObject.TryGetComponent(out IExpPoint expPoint))
            {
                //тут добавление в лвл кол-во поинтов expPoint.Value
                expPoint.Despawn();
            }
        }
    }
}