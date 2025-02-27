using Zenject;

public class EnemyPool : Pool<Enemy>
{
    public EnemyPool(DiContainer container) : base(container) { }

    protected override Enemy Create(IData<Enemy> data)
    {
        Enemy newEnemy = _container.InstantiatePrefabForComponent<Enemy>(data.Prefab);
        newEnemy.Initialize(data, this);
        return newEnemy;
    }

    protected override Enemy GetInitializedEntity(IData<Enemy> data, Enemy entity)
    {
        entity.Initialize(data, this);
        return entity;
    }
}
