using UnityEngine;
using Zenject;

public class ProjectileInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ProjectilePool>().AsSingle();
    }
}