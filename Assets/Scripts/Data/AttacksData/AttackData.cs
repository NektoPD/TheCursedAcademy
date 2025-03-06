using UnityEngine;

public abstract class AttackData : ScriptableObject
{
    [SerializeField] private string _nameInAnimator;
    [SerializeField] private float _cooldown;
    [SerializeField] private float _attackRange;

    public string NameInAnimator => _nameInAnimator;

    public float Cooldown => _cooldown;

    public float AttackRange => _attackRange;
}
