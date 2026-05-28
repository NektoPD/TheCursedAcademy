using Data;
using Data.ExpPointsData;
using Difficulties;
using Pools;
using UnityEngine;
using Utils;
using Zenject;

namespace PickableItems
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
        private XpWaveScaler _xpWaveScaler;

        public int Value => _point;

        public ExpPoint Prefab => _prefab;

        [Inject]
        private void Construct(XpWaveScaler xpWaveScaler)
        {
            _xpWaveScaler = xpWaveScaler;
        }

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
            int basePoint = expPointData.Point;
            _point = _xpWaveScaler != null ? _xpWaveScaler.Scale(basePoint) : basePoint;
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
