using System.Collections.Generic;
using Zenject;

public abstract class Pool<T> where T: IPoolEntity
{
    protected readonly DiContainer _container;
    protected readonly Queue<T> _pool = new();
    protected int _count;

    public Pool(DiContainer container)
    {
        _container = container;
    }

    public int Active => _count - _pool.Count;

    public void ReturnEntity(T entity)
    {
        entity.Despawn();
        _pool.Enqueue(entity);
    }

    public T Get(IData<T> data)
    {
        if (_pool.Count > 0)
        {
            var entity = _pool.Dequeue();
            entity.ResetEntity();
            return entity;
        }

        var newEntity = Create(data);
        _count++;
        return newEntity;
    }

    protected abstract T Create(IData<T> data);
}
