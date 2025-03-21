using Data;
using Utils;
using Pools;
using UnityEngine;

namespace ExpPoints
{
    [RequireComponent(typeof(SpriteRenderer), typeof(ResizeCollider))]
    public class ExpPoint : MonoBehaviour, IPoolEntity, IExpPoint
    {
        private int _point;
        private ExpPointPool _pool;
        private SpriteRenderer _spriteRenderer;
        private ResizeCollider _resizeCollider;

        public int Point => _point;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _resizeCollider = GetComponent<ResizeCollider>();
        }

        public void Initialize(IData<ExpPoint> data, ExpPointPool pool)
        {
            ExpPointData expPointData = data as ExpPointData;

            _point = expPointData.Point;
            _pool = pool;
            _spriteRenderer.sprite = expPointData.Sprite;
            _resizeCollider.Resize();
        }

        public void Despawn()
        {
            gameObject.SetActive(false);
            _pool.ReturnEntity(this);
        }

        public void ResetEntity() => gameObject.SetActive(true);
    }
}
