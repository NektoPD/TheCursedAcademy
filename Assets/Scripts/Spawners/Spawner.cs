using UnityEngine;

public abstract class Spawner<T> : MonoBehaviour where T : IPoolEntity
{
    protected Pool<T> _pool;

    public int ActiveEnemy => _pool.Active;

    public T Spawn(IData<T> data)
    {
        var entity = _pool.Get(data);
        entity.ResetEntity();

        return entity;
    }
}
