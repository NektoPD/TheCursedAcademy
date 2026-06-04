using System.Collections;
using UnityEngine;

namespace EnemyLogic.BossArenaLogic
{
    [RequireComponent(typeof(EnemyDamageTaker))]
    public class BossArena : MonoBehaviour
    {
        [SerializeField] private Vector2 _arenaSize = new(16f, 9f);
        [SerializeField] private float _teleportDistanceFromCenter = 3f;
        [SerializeField] private float _wallThickness = 0.5f;
        [SerializeField] private Transform _wallPrefab;
        [SerializeField] private Camera _bossCamera;

        private Transform[] _arenaWalls;
        private Bounds _arenaBounds;
        private Camera _mainCamera;
        private EnemyDamageTaker _enemyDamageTaker;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _enemyDamageTaker = GetComponent<EnemyDamageTaker>();
        }

        void Start()
        {
            TeleportBossToCenter();
            _mainCamera.gameObject.SetActive(false);
            _bossCamera.gameObject.SetActive(true);
            _bossCamera.transform.SetParent(null);
            CalculateArenaBounds();
            BuildArena();
            _enemyDamageTaker.Health.Died += DeactivateArea;
        }

        private void TeleportBossToCenter()
        {
            Vector3 teleportPosition = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, _mainCamera.nearClipPlane)) + Vector3.left * _teleportDistanceFromCenter;

            transform.position = teleportPosition;
        }

        private void CalculateArenaBounds()
        {
            Vector3 bossPosition = transform.position;
            _arenaBounds = new Bounds(bossPosition, _arenaSize);
        }

        private void BuildArena()
        {
            _arenaWalls = new Transform[4];
            Vector3 center = _arenaBounds.center;
            Vector3 size = _arenaBounds.size;

            _arenaWalls[0] = CreateWall(new Vector3(center.x, center.y + size.y / 2, 0), new Vector3(size.x, _wallThickness, 1));
            _arenaWalls[1] = CreateWall(new Vector3(center.x, center.y - size.y / 2, 0), new Vector3(size.x, _wallThickness, 1));
            _arenaWalls[2] = CreateWall(new Vector3(center.x - size.x / 2, center.y, 0), new Vector3(_wallThickness, size.y, 1));
            _arenaWalls[3] = CreateWall(new Vector3(center.x + size.x / 2, center.y, 0), new Vector3(_wallThickness, size.y, 1));
        }

        private Transform CreateWall(Vector3 position, Vector3 scale)
        {
            Transform wall = Instantiate(_wallPrefab, position, Quaternion.identity);
            wall.transform.localScale = scale;
            return wall;
        }

        private void DeactivateArea()
        {
            _bossCamera.gameObject.SetActive(false);
            _mainCamera.gameObject.SetActive(true);
            foreach (var wall in _arenaWalls)
                Destroy(wall.gameObject);
        }
    }
}