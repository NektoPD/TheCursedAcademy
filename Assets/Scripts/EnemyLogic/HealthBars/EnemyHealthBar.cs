using UnityEngine;
using UnityEngine.UI;

namespace EnemyLogic.HealthBars
{
    public class EnemyHealthBar : HealthBar
    {
        [SerializeField] private Slider _hpBar;

        private void Awake()
        {
            Bar = _hpBar;
        }
    }
}
