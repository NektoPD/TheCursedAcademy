using Data;
using Data.ExpPointsData;
using Pools;
using UnityEngine;
using Utils;

namespace ExpPoints
{
    [RequireComponent(typeof(SpriteRenderer), typeof(ResizeCollider), typeof(Animator))]
    public class ExpPoint : MonoBehaviour, IPoolEntity, IExpPoint
    {
        private int _point;
        private ExpPointPool _pool;
        private SpriteRenderer _spriteRenderer;
        private ResizeCollider _resizeCollider;
        private Animator _animator;

        public int Value => _point;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _resizeCollider = GetComponent<ResizeCollider>();
            _animator = GetComponent<Animator>();
        }

        public void Initialize(IData<ExpPoint> data, ExpPointPool pool)
        {
            ExpPointData expPointData = data as ExpPointData;

            _point = expPointData.Point;
            _pool = pool;
            _spriteRenderer.sprite = expPointData.Sprite;
            _resizeCollider.Resize();
            _animator.runtimeAnimatorController = expPointData.AnimatorController;
        }

        public void Despawn()
        {
            gameObject.SetActive(false);
            _pool.ReturnEntity(this);
        }

        public void ResetEntity() => gameObject.SetActive(true);
    }
}
