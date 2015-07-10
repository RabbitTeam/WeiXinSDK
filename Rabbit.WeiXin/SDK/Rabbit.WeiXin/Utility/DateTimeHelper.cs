using System;

namespace Rabbit.WeiXin.Utility
{
    /// <summary>
    /// 日期时间助手。
    /// </summary>
    public class DateTimeHelper
    {
        /// <summary>
        /// 时间戳的起始时间。
        /// </summary>
        public static DateTime StartTime = new DateTime(1970, 1, 1);

        /// <summary>
        /// 根据时间戳得到具体的时间信息。
        /// </summary>
        /// <param name="timestamp">时间戳。</param>
        /// <returns>时间信息。</returns>
        public static DateTime GetTimeByTimeStamp(ulong timestamp)
        {
            return StartTime.AddSeconds(timestamp);
        }

        /// <summary>
        /// 根据时间戳字符串得到时间信息。
        /// </summary>
        /// <param name="timestampString">时间戳字符串。</param>
        /// <returns>时间信息。</returns>
        public static DateTime GetTimeByTimeStampString(string timestampString)
        {
            ulong timestamp;
            if (!ulong.TryParse(timestampString, out timestamp))
                throw new ArgumentNullException("timestampString", "不是一个有效的时间戳。");
            return StartTime.AddSeconds(timestamp);
        }

        /// <summary>
        /// 根据时间信息得到时间戳。
        /// </summary>
        /// <param name="dateTime">时间信息。</param>
        /// <returns>时间戳。</returns>
        public static long GetTimeStampByTime(DateTime dateTime)
        {
            return Convert.ToInt64((dateTime - StartTime).TotalSeconds);
        }
    }
}