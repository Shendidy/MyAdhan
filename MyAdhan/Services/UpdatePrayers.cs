using Microsoft.Extensions.Logging;
using MyAdhan.Scheduler.Enums;
using MyAdhan.Scheduler.Models;
using Newtonsoft.Json;

namespace MyAdhan.Scheduler.Services
{
    public class UpdatePrayers : IUpdatePrayers
    {
        private readonly ILogger<UpdatePrayers>? _logger;
        public IPrayers _prayers;
        public ICallPrayers _callPrayers;
        public UpdatePrayers(ILogger<UpdatePrayers> logger
            , IPrayers prayers
            , ICallPrayers callPrayers)
        {
            _logger = logger;
            _prayers = prayers;
            _callPrayers = callPrayers;
        }

        public void Update(string json)
        {
            if (_prayers == null)
            {
                _logger.LogError($"No prayers object is created yet!");
            }
            else
            {
                //Use these values for testing
                //string jsonFilePath = @".\JsonFiles\AdhanAPIResponse.json";
                //json = File.ReadAllText(jsonFilePath);

                dynamic details = JsonConvert.DeserializeObject<dynamic>(json);

                int hijriMonthNumber = Int32.Parse(details.SelectToken(GetConfig("PrayersApiResponse, HijriMonthNumber")).ToString());
                string hijriMonth = ((ArabicMonths)hijriMonthNumber).ToString();
                string hijriYear = details.SelectToken(GetConfig("PrayersApiResponse, HijriYear"));
                string hijriDay = details.SelectToken(GetConfig("PrayersApiResponse, HijriDay"));

                _prayers.Fajr = details.SelectToken(GetConfig("PrayersApiResponse, Fajr"));
                //_prayers.Fajr = DateTime.Now.AddMinutes(2).ToString("hh:mm"); //use for docker container tests
                _prayers.Dhuhr = details.SelectToken(GetConfig("PrayersApiResponse, Dhuhr"));
                _prayers.Asr = details.SelectToken(GetConfig("PrayersApiResponse, Asr"));
                _prayers.Maghrib = details.SelectToken(GetConfig("PrayersApiResponse, Maghrib"));
                _prayers.Isha = details.SelectToken(GetConfig("PrayersApiResponse, Isha"));
                _prayers.DateHijri = $"{hijriDay} {hijriMonth} {hijriYear}";

                var datePath = GetConfig("PrayersApiResponse, Date");
                var dateFormat = GetConfig("PrayersApiResponse, DateFormat");
                var date = details.SelectToken(datePath).ToString();
                _prayers.Date = DateOnly.ParseExact(date, dateFormat);

                LogPrayerTimes();

                _callPrayers.CallEndpoints();
            }
        }

        private void LogPrayerTimes()
        {
            Console.WriteLine($"  {String.Concat(Enumerable.Repeat("*", 35))}");
            Console.WriteLine($"  {GetLog(_prayers.Date.ToString("dd MMMM yyyy"))}");
            Console.WriteLine($"  {GetLog(_prayers.DateHijri)}");
            Console.WriteLine($"  * Fajr:     {_prayers.Fajr}                 *");
            Console.WriteLine($"  * Dhuhr:    {_prayers.Dhuhr}                 *");
            Console.WriteLine($"  * Asr:      {_prayers.Asr}                 *");
            Console.WriteLine($"  * Maghrib:  {_prayers.Maghrib}                 *");
            Console.WriteLine($"  * Isha:     {_prayers.Isha}                 *");
            Console.WriteLine($"  {String.Concat(Enumerable.Repeat("*", 35))}");
        }

        private string GetLog(string log)
        {
            int length = 33;
            string finalLog = string.Empty;

            int logLength = log.Length;
            int leftGap = Convert.ToInt32(Math.Floor((length - logLength)/2m));
            int rightGap = length - logLength - leftGap;

            string left = String.Concat(Enumerable.Repeat(" ", leftGap));
            string right = String.Concat(Enumerable.Repeat(" ", rightGap));


            return $"*{left}{log}{right}*";
        }

        private string GetConfig(string path)
            => ConfigurationManager.GetConfigValue(path);
    }
}
