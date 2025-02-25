using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class EnemyPool
{
    private readonly DiContainer _container;
    private readonly Queue<Enemy> _pool = new();
    private int _countEnemy;

    public EnemyPool(DiContainer container)
    {
        _container = container;
    }

    public int Active => _countEnemy - _pool.Count;

    public Enemy GetEnemy(EnemyData enemyData, Transform target)
    {
        if (_pool.Count > 0)
        {
            var enemy = _pool.Dequeue();
            enemy.ResetEnemy();
            return enemy;
        }

        var newEnemy = _container.InstantiatePrefabForComponent<Enemy>(enemyData.Prefab);
        newEnemy.Initialize(enemyData, target, this);
        _countEnemy++;
        return newEnemy;
    }

    public void ReturnEnemy(Enemy enemy)
    {
        enemy.Despawn();
        _pool.Enqueue(enemy);
    }
}
