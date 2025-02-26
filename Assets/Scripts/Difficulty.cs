using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Difficulty : MonoBehaviour
{
    [SerializeField] private float _cooldown = 1;
    [SerializeField] private int _maxEnemy = 100;
    [SerializeField] private float _offset;

    private EnemySpawner _enemySpawner;
    private List<EnemyData> _enemyDataList;
    private Coroutine _coroutine;

    private bool _canSpawn = true;

    [Inject]
    public void Construct(EnemySpawner enemySpawner, List<EnemyData> enemyDataList)
    {
        _enemySpawner = enemySpawner;
        _enemyDataList = enemyDataList;
    }

    private void OnEnable()
    {
        _coroutine = StartCoroutine(Cooldown());
    }

    private void OnDisable()
    {
        StopCoroutine(_coroutine);
    }

    private void Update()
    {
        if (_canSpawn && _enemySpawner.ActiveEnemy <= _maxEnemy)
        {
            _canSpawn = false;
            SpawnRandomEnemy();

            _coroutine = StartCoroutine(Cooldown());
        }
    }

    private void SpawnRandomEnemy()
    {
        if (_enemyDataList.Count > 0)
        {
            var randomData = _enemyDataList[Random.Range(0, _enemyDataList.Count)];
            Enemy enemy = _enemySpawner.Spawn(randomData);
            enemy.transform.position = GetRandomPositionOutsideCamera();
        }
    }

    private Vector3 GetRandomPositionOutsideCamera()
    {
        Camera camera = Camera.main;

        Side[] pool = new Side[] { Side.Top, Side.Bottom, Side.Right, Side.Left };
        Side side = (Side)pool.GetValue(Random.Range(0, pool.Length));

        Vector3 viewportPosition = Vector3.zero;

        switch (side)
        {
            case Side.Top:
                viewportPosition = new Vector3(Random.Range(0f, 1f), 1 + _offset, camera.nearClipPlane);
                break;

            case Side.Bottom:
                viewportPosition = new Vector3(Random.Range(0f, 1f), -_offset, camera.nearClipPlane);
                break;

            case Side.Left:
                viewportPosition = new Vector3(-_offset, Random.Range(0f, 1f), camera.nearClipPlane);
                break;

            case Side.Right:
                viewportPosition = new Vector3(1 + _offset, Random.Range(0f, 1f), camera.nearClipPlane);
                break;
        }

        return camera.ViewportToWorldPoint(viewportPosition);
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(_cooldown);

        _canSpawn = true;
    }
}
