using System.Collections.Generic;
using Items.Enums;

public class StatModifiers
{
    private readonly Dictionary<StatVariations, float> _mult = new();

    public float GetMult(StatVariations v) => _mult.TryGetValue(v, out var m) ? m : 1f;
    public void Multiply(StatVariations v, float k) => _mult[v] = GetMult(v) * k;
}