using System.Collections.Generic;

[System.Serializable]
public class DifficultyData
{
    private float _playTime;
    private List<int> _enemyIds;

    public DifficultyData(float playTime, List<int> enemyIds)
    {
        _playTime = playTime;
        _enemyIds = enemyIds;
    }

    public float PlayTime => _playTime;

    public IReadOnlyList<int> EnemyIds => _enemyIds;  
}
