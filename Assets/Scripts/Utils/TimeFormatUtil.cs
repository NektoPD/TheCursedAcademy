using System;

public static class TimeFormatUtil
{
    public static string FormatLikeTimeView(TimeSpan timeSpan)
    {
        if (timeSpan.TotalHours >= 1)
            return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        return string.Format("{0}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }
}