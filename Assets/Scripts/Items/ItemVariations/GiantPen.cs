using System.Collections.Generic;
using Items.BaseClass;
using Items.Pools;
using UnityEngine;

namespace Items.ItemVariations
{
    public class GiantPen : Item
    {
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private float _projectileSpeed = 10f;
        [SerializeField] private float _projectileLifetime = 2f;
        [SerializeField] private float _attackWidth = 3f;
        [SerializeField] private float _attackDistance = 5f;
        [SerializeField] private int _initialPoolSize = 3;

        private int _level = 1;
        private float _damageMultiplier = 1f;
        private float _widthMultiplier = 1f;
        private ProjectilePool _projectilePool;
        private Transform _projectilesContainer;

        private void Awake()
        {
            _projectilesContainer = new GameObject("GiantPenProjectiles").transform;
            _projectilesContainer.SetParent(transform);

            // Создаем пул проджектайлов
            _projectilePool = _projectilesContainer.gameObject.AddComponent<ProjectilePool>();
        }

        private void Start()
        {
            // Инициализируем пул с нашим префабом и заданным начальным размером
            _projectilePool.Initialize(_projectilePrefab, _initialPoolSize);
        }

        protected override void PerformAttack()
        {
            // Получаем проджектайл из пула
            GameObject projectile = _projectilePool.GetFromPool(transform.position, Quaternion.identity);

            // Настраиваем размер проджектайла
            projectile.transform.localScale = new Vector3(_attackWidth * _widthMultiplier, 1f, 1f);

            // Получаем или добавляем компонент урона
            ProjectileDamage damage = projectile.GetComponent<ProjectileDamage>();
            if (damage == null)
            {
                damage = projectile.AddComponent<ProjectileDamage>();
            }

            // Инициализируем компонент урона
            damage.Initialize(Data.Damage * _damageMultiplier, transform.parent.gameObject);

            // Сбрасываем список пораженных врагов при повторном использовании
            damage.ClearHitEnemies();

            // Запускаем движение проджектайла
            StartCoroutine(MoveProjectile(projectile, transform.right, _projectileSpeed, _projectileLifetime));
        }

        protected override void LevelUp()
        {
            _level++;

            // Увеличиваем урон с каждым уровнем
            _damageMultiplier = 1f + (_level - 1) * 0.25f;

            // Каждые 3 уровня увеличиваем ширину атаки
            if (_level % 3 == 0)
            {
                _widthMultiplier += 0.2f;
            }

            // Каждые 5 уровней уменьшаем кулдаун (максимум на 50%)
            if (_level % 5 == 0 && _level <= 25)
            {
                Data.Cooldown *= 0.9f;
            }
        }

        private IEnumerator MoveProjectile(GameObject projectile, Vector3 direction, float speed, float lifetime)
        {
            float timer = 0f;

            while (timer < lifetime && projectile != null && projectile.activeSelf)
            {
                projectile.transform.position += direction * speed * Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }

            // Возвращаем проджектайл в пул
            if (projectile != null && projectile.activeSelf)
            {
                _projectilePool.ReturnToPool(projectile);
            }
        }

        // Очистка ресурсов при уничтожении объекта
        private void OnDestroy()
        {
            // Проджектайлы и пул будут уничтожены автоматически вместе с контейнером
            if (_projectilesContainer != null)
            {
                Destroy(_projectilesContainer.gameObject);
            }
        }
    }
    
    public class ProjectileDamage : MonoBehaviour
    {
        private float _damage;
        private GameObject _owner;
        private HashSet<GameObject> _hitEnemies = new HashSet<GameObject>();
        
        public void Initialize(float damage, GameObject owner)
        {
            _damage = damage;
            _owner = owner;
        }
        
        // Метод для сброса списка пораженных врагов при повторном использовании
        public void ClearHitEnemies()
        {
            _hitEnemies.Clear();
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy") && !_hitEnemies.Contains(collision.gameObject))
            {
                // Добавляем в список пораженных врагов, чтобы не поражать одного врага несколько раз
                _hitEnemies.Add(collision.gameObject);
                
                // Наносим урон врагу
                IDamageable damageable = collision.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(_damage);
                }
            }
        }
    }
}