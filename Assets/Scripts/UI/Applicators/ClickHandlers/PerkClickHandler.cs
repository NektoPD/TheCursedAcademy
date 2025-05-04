using Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Applicators.ClickHandlers
{
    public class PerkClickHandler : BaseClickHandler<PerkVisualData>
    {
        [SerializeField] private Image[] _levels;
        [SerializeField] private Sprite _on;
        [SerializeField] private PerkApplicator _applicator;

        private Queue<Image> _currentLevels;

        private void Start()
        {
            _currentLevels = new Queue<Image>();

            foreach (var level in _levels)
                _currentLevels.Enqueue(level);
        }

        private void OnEnable()
        {
            _applicator.Buyed += Up;
        }

        private void OnDisable()
        {
            _applicator.Buyed -= Up;
        }

        public void Up(PerkVisualData data)
        {
            if (data != Data)
                return;

            if (_currentLevels.Count == 0)
                return;

            Image level = _currentLevels.Dequeue();
            level.sprite = _on;
        }
    }
}