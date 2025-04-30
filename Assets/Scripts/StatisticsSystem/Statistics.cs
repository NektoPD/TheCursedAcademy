using System;
using System.Collections.Generic;

namespace StatistiscSystem
{
    public class Statistics
    {
        public Statistics(int totalScore, int level, int enemysKills, int coins, TimeSpan liveTime, List<ItemStatistics> items)
        {
            TotalScore = totalScore;
            Level = level;
            EnemysKills = enemysKills;
            Coins = coins;
            LiveTime = liveTime;
            Items = items;
        }

        public int TotalScore { get; private set; }

        public int Level { get; private set; }

        public int EnemysKills { get; private set; }

        public int Coins { get; private set; }

        public TimeSpan LiveTime { get; private set; }

        public List<ItemStatistics> Items { get; private set; }
    }
}