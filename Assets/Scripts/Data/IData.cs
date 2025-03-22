using Pools;

namespace Data
{
    public interface IData<T> where T : IPoolEntity
    {
        public T Prefab { get; }
    }
}
