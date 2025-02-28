using UnityEngine;

[CreateAssetMenu(fileName = "ExpPointData", menuName = "ExpPoint/ExpPointData", order = 1)]
public class ExpPointData : ScriptableObject, IData<ExpPoint>
{
    [SerializeField] private int _point;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private ExpPoint _prefab;

    public int Point => _point;

    public Sprite Sprite => _sprite;

    public ExpPoint Prefab => _prefab;
}
