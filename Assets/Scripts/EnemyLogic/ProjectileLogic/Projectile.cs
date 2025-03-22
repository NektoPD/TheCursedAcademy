using Data;
using Data.ProjectilesData;
using Pools;
using UnityEngine;
using Utils;

namespace EnemyLogic.ProjectileLogic
{
    [RequireComponent(typeof(ProjectileView), typeof(CollisionDetecter), typeof(ProjectileMover))]
    public class Projectile : MonoBehaviour, IPoolEntity
    {
        private ProjectileView _view;
        private ProjectilePool _pool;
        private CollisionDetecter _detecter;
        private ProjectileMover _mover;
        private ProjectileData _data;

        private void OnEnable()
        {
            _view = GetComponent<ProjectileView>();
            _detecter = GetComponent<CollisionDetecter>();
            _mover = GetComponent<ProjectileMover>();
        }

        public void Initialize(IData<Projectile> data, ProjectilePool pool)
        {
            ProjectileData projectileData = data as ProjectileData;

            _pool = pool;
            _data = projectileData;
            _view.Initialize(projectileData.Sprite, projectileData.AnimatorController);
        }

        public void SetDamage(float damage) => _detecter.Initialize(damage);

        public void SetDirection(Vector2 direction) => _mover.Initialize(direction, _data.Speed);

        public void Despawn()
        {
            gameObject.SetActive(false);
            _pool.ReturnEntity(this);
        }

        public void ResetEntity() => gameObject.SetActive(true);
    }
}