using Pools;
using UnityEngine;
using WalletSystem.MoneyLogic;
using Zenject;

public class MoneyInstaller : MonoInstaller
{
    [SerializeField] private Money _prefab;

    public override void InstallBindings()
    {
        Container.Bind<MoneyPool>().AsSingle().WithArguments(_prefab);
    }
}
