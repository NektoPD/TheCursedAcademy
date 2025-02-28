using System.Linq;
using Zenject;

public class EnemyPool : Pool<Enemy>
{
    public EnemyPool(DiContainer container) : base(container) { }

    protected override Enemy Create(IData<Enemy> data)
    {
        Enemy newEnemy = Container.InstantiatePrefabForComponent<Enemy>(data.Prefab);
        newEnemy.Initialize(data, this);
        return newEnemy;
    }
    protected override bool TryGetEntityInPool(IData<Enemy> data, out Enemy entity)
    {
        entity = null;

        if(data.Prefab is MeleeEnemy)
            entity = EntityPool.FirstOrDefault(entity => entity is MeleeEnemy);

        if (data.Prefab is RangeEnemy)
            entity = EntityPool.FirstOrDefault(entity => entity is RangeEnemy);

        if (entity != null)
            entity.Initialize(data, this);

        return entity != null;
    }
}
