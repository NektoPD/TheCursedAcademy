using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Difficulty : MonoBehaviour
{
    [SerializeField] private float _cooldown = 1;
    [SerializeField] private int _maxEnemy = 100;

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
            _enemySpawner.SpawnEnemy(randomData);
        }
    }


    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(_cooldown);

        _canSpawn = true;
    }
}
