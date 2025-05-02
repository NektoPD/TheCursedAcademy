using System;

namespace StatistiscSystem
{
    public interface IStatisticsTransmitter
    {
        public event Action<Statistics> StatisticCollected;
    }
}