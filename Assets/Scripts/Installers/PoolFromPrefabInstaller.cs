using PickableItems;
using Pools.FromPrefab;
using UnityEngine;
using Zenject;

public class PoolFromPrefabInstaller : MonoInstaller
{
    [SerializeField] private Money _moneyPrefab;
    [SerializeField] private Heal _healPrefab;
    [SerializeField] private Magnet _magnetPrefab;

    public override void InstallBindings()
    {
        Container.Bind<MoneyPool>().AsSingle().WithArguments(_moneyPrefab);
        Container.Bind<HealPool>().AsSingle().WithArguments(_healPrefab);
        Container.Bind<MagnetPool>().AsSingle().WithArguments(_magnetPrefab);
    }
}
