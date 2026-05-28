using System;
using UnityEngine;

namespace Difficulties
{
    public class XpWaveScaler
    {
        private float _baseMultiplier = 1f;
        private float _growthPerWave = 1.1f;
        private int _waveIndex;

        public int WaveIndex => _waveIndex;

        public void Configure(float baseMultiplier, float growthPerWave)
        {
            _baseMultiplier = Mathf.Max(0f, baseMultiplier);
            _growthPerWave = Mathf.Max(0f, growthPerWave);
        }

        public void SetWaveIndex(int waveIndex)
        {
            _waveIndex = Math.Max(0, waveIndex);
        }

        public int Scale(int baseXp)
        {
            if (baseXp <= 0)
                return 0;

            double multiplier = _baseMultiplier * Math.Pow(_growthPerWave, _waveIndex);
            int scaled = (int)Math.Round(baseXp * multiplier, MidpointRounding.AwayFromZero);
            return Math.Max(1, scaled);
        }
    }
}

