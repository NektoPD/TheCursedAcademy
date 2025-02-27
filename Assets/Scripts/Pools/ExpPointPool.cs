using Zenject;

public class ExpPointPool : Pool<ExpPoint>
{
    public ExpPointPool(DiContainer container) : base(container) { }

    protected override ExpPoint Create(IData<ExpPoint> data)
    {
        ExpPoint newExpPoint = _container.InstantiatePrefabForComponent<ExpPoint>(data.Prefab);
        newExpPoint.Initialize(data, this);
        return newExpPoint;
    }

    protected override ExpPoint GetInitializedEntity(IData<ExpPoint> data, ExpPoint entity)
    {
        entity.Initialize(data, this);
        return entity;
    }
}
