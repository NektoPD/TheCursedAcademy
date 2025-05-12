using Data;
using Data.ExpPointsData;
using Pools;
using UnityEngine;
using Utils;

namespace ExpPoints
{
    [RequireComponent(typeof(SpriteRenderer), typeof(ResizeCollider), typeof(Animator))]
    public class ExpPoint : MonoBehaviour, IPoolEntity, IPickable
    {
        private int _point;
        private ExpPointPool _pool;
        private SpriteRenderer _spriteRenderer;
        private ResizeCollider _resizeCollider;
        private Animator _animator;
        private ExpPoint _prefab;

        public int Value => _point;

        public ExpPoint Prefab => _prefab;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _resizeCollider = GetComponent<ResizeCollider>();
            _animator = GetComponent<Animator>();
        }

        public void Initialize(IData<ExpPoint> data, ExpPointPool pool)
        {
            ExpPointData expPointData = data as ExpPointData;

            _prefab = data.Prefab;
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
