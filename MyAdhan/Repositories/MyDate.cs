namespace MyAdhan.Scheduler.Repositories
{
    public class MyDate : IMyDate
    {
        public DateTime GetNow() => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, GetConfig("TimeZone"));

        public DateOnly GetToday() => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, GetConfig("TimeZone")));

        private static string GetConfig(string path)
            => ConfigurationManager.GetConfigValue(path);
    }
}
