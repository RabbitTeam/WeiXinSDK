using System;

namespace ChunSun.PublicPlatform.Services.Utility
{
    public class DateTimeHelper
    {
        private static readonly DateTime StartTime = new DateTime(1970, 1, 1);

        public static DateTime GetTimeByTimeStamp(ulong timestamp)
        {
            return StartTime.AddSeconds(timestamp);
        }

        public static DateTime GetTimeByTimeStampString(string timestampString)
        {
            long timestamp;
            if (!long.TryParse(timestampString, out timestamp))
                throw new ArgumentNullException("timestampString", "不是一个有效的时间戳。");
            return StartTime.AddSeconds(timestamp);
        }

        public static long GetTimeStampByTime(DateTime dateTime)
        {
            return Convert.ToInt64((dateTime - StartTime).TotalSeconds);
        }
    }
}