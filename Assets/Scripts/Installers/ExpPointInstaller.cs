using Zenject;

public class ExpPointInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ExpPointPool>().AsSingle();
    }
}