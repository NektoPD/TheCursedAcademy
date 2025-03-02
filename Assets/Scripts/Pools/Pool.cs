using System.Collections.Generic;
using System.Linq;
using Zenject;

public abstract class Pool<T> where T: IPoolEntity
{
    protected readonly DiContainer Container;
    protected readonly List<T> EntityPool = new();

    private int _count;

    public Pool(DiContainer container)
    {
        Container = container;
    }

    public int Active => _count - EntityPool.Count;

    public void ReturnEntity(T entity) => EntityPool.Add(entity);

    public T Get(IData<T> data)
    {
        if (EntityPool.Count > 0)
        {
            T entity = EntityPool.First();
            entity = Initialize(data, entity);
            EntityPool.Remove(entity);

            return entity;
        }

        _count++;

        var newEntity = Create(data);
        newEntity = Initialize(data, newEntity);
        return newEntity;
    }

    protected abstract T Initialize(IData<T> data, T entity);

    protected abstract T Create(IData<T> data);
}
