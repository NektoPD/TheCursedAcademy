using System.Linq;
using Zenject;

public class ExpPointPool : Pool<ExpPoint>
{
    public ExpPointPool(DiContainer container) : base(container) { }

    protected override ExpPoint Create(IData<ExpPoint> data)
    {
        ExpPoint newExpPoint = Container.InstantiatePrefabForComponent<ExpPoint>(data.Prefab);
        newExpPoint.Initialize(data, this);
        return newExpPoint;
    }

    protected override bool TryGetEntityInPool(IData<ExpPoint> data, out ExpPoint entity)
    {
        entity = EntityPool.First();
        entity.Initialize(data, this);
        return true;
    }
}
