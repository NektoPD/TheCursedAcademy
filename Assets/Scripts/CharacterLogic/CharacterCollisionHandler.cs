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
                //_wallet.AddMoney(money.Value); ��� � �����, ��� ����� ��������� ����-�� ���-�� ��������� �����, � ����� � ����� ��� ������ ����� ���������� �� � ������, ����� ������ ���� ����� �� ���� ����� � ������ ���������� �� �����
                money.Despawn();
            }

            if (collision.gameObject.TryGetComponent(out IExpPoint expPoint))
            {
                GotExpPoint?.Invoke(expPoint.Value);
                //��� ���������� � ��� ���-�� ������� expPoint.Value
                expPoint.Despawn();
            }
        }
    }
}