using UnityEngine;

public class ExpPoint : MonoBehaviour, IPoolEntity, IExpPoint
{
    private int _point;
    private ExpPointPool _pool;

    public int Point => _point;

    public void Initialize(IData<ExpPoint> data, ExpPointPool pool)
    {
        ExpPointData expPointData = data as ExpPointData;

        _point = expPointData.Point;
        _pool = pool;
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
        _pool.ReturnEntity(this);
    }

    public void ResetEntity() => gameObject.SetActive(true);
}
