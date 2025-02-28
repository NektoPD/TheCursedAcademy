using System.Collections.Generic;
using Zenject;

public abstract class Pool<T> where T: IPoolEntity
{
    protected readonly DiContainer Container;
    protected readonly List<T> EntityPool = new();
    protected int Count;

    public Pool(DiContainer container)
    {
        Container = container;
    }

    public int Active => Count - EntityPool.Count;

    public void ReturnEntity(T entity) => EntityPool.Add(entity);

    public T Get(IData<T> data)
    {
        if (EntityPool.Count != 0 && TryGetEntityInPool(data, out T entity))
        {
            entity.ResetEntity();
            return entity;
        }

        var newEntity = Create(data);
        newEntity.ResetEntity();
        Count++;
        return newEntity;
    }

    protected abstract T Create(IData<T> data);

    protected abstract bool TryGetEntityInPool(IData<T> data, out T entity);
}
