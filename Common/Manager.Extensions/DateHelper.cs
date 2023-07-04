namespace Manager.Extensions
{
    public static class DateHelper
    {
        /// <summary>
        /// 计算 DateTime1 和  DateTime2 的时间差[绝对值]
        /// </summary>
        /// <param name="DateTime1">第一个日期和时间</param>
        /// <param name="DateTime2">第二个日期和时间</param>
        /// <returns></returns>
        public static int SecondDiff(DateTime DateTime1, DateTime DateTime2)
        {
            TimeSpan ts1 = new(DateTime1.Ticks);
            TimeSpan ts2 = new(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            var dayDiff = ts.Days;
            var hourDiff = ts.Hours;
            var minuteDiff = ts.Minutes;
            var secondDiff = ts.Seconds;
            var diff = secondDiff + minuteDiff * 60 + hourDiff * 60 * 60 + dayDiff * 24 * 60 * 60;
            return diff;
        }

        public static long ConvertDateTimeToLong(DateTime? time)
        {
            if (time == null)
            {
                throw new ArgumentNullException(nameof(time));
            }
            DateTime dd = new(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan ts = (time.Value - dd);
            return (long)ts.TotalMilliseconds;
        }

        public static DateTime ConvertStringToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new(lTime);
            return dtStart.Add(toNow);
        }
    }
}