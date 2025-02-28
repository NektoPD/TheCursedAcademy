using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Difficulty : MonoBehaviour
{
    [SerializeField] private float _cooldown = 1;
    [SerializeField] private int _maxEnemy = 100;
    [SerializeField] private float _offset;

    private EnemyPool _enemyPool;
    private List<EnemyData> _enemyDataList;
    private Coroutine _coroutine;

    private bool _canSpawn = true;
    private int _id = 1;

    [Inject]
    public void Construct(EnemyPool enemyPool, List<EnemyData> enemyDataList)
    {
        _enemyPool = enemyPool;
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
        if (_canSpawn && _enemyPool.Active < _maxEnemy)
        {
            _canSpawn = false;
            Enemy enemy = GetNextIdEnemy();
            enemy.transform.position = GetRandomPositionOutsideCamera();

            _coroutine = StartCoroutine(Cooldown());
        }
    }

    private Enemy GetRandomEnemy()
    {
        var randomData = _enemyDataList[Random.Range(0, _enemyDataList.Count)];
        return _enemyPool.Get(randomData);
    }

    private Enemy GetNextIdEnemy()
    {
        if (_id > _enemyDataList.Count)
            _id = 1;

        foreach (var data in _enemyDataList)
        {
            if (data.Id == _id)
            {
                _id++;
                return _enemyPool.Get(data);
            }
        }

        return GetRandomEnemy();
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
