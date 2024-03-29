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

                int hijriMonthNumber = Int32.Parse(details.SelectToken(getConfig("PrayersApiResponse, HijriMonthNumber")).ToString());
                string hijriMonth = ((ArabicMonths)hijriMonthNumber).ToString();
                string hijriYear = details.SelectToken(getConfig("PrayersApiResponse, HijriYear"));
                string hijriDay = details.SelectToken(getConfig("PrayersApiResponse, HijriDay"));

                _prayers.Fajr = details.SelectToken(getConfig("PrayersApiResponse, Fajr"));
                //_prayers.Fajr = DateTime.Now.AddMinutes(2).ToString("hh:mm"); //use for docker container tests
                _prayers.Dhuhr = details.SelectToken(getConfig("PrayersApiResponse, Dhuhr"));
                _prayers.Asr = details.SelectToken(getConfig("PrayersApiResponse, Asr"));
                _prayers.Maghrib = details.SelectToken(getConfig("PrayersApiResponse, Maghrib"));
                _prayers.Isha = details.SelectToken(getConfig("PrayersApiResponse, Isha"));
                _prayers.DateHijri = $"{hijriDay} {hijriMonth} {hijriYear}";

                var datePath = getConfig("PrayersApiResponse, Date");
                var dateFormat = getConfig("PrayersApiResponse, DateFormat");
                var date = details.SelectToken(datePath).ToString();
                _prayers.Date = DateOnly.ParseExact(date, dateFormat);

                Console.WriteLine($"***********************************");
                Console.WriteLine($"*          {_prayers.Date}");
                Console.WriteLine($"*       {_prayers.DateHijri}");
                Console.WriteLine($"* Fajr:     {_prayers.Fajr}");
                Console.WriteLine($"* Dhuhr:    {_prayers.Dhuhr}");
                Console.WriteLine($"* Asr:      {_prayers.Asr}");
                Console.WriteLine($"* Maghrib:  {_prayers.Maghrib}");
                Console.WriteLine($"* Isha:     {_prayers.Isha}");
                Console.WriteLine($"***********************************");

                _callPrayers.CallEndpoints();
            }
        }

        private string getConfig(string path)
            => ConfigurationManager.GetConfigValue(path);
    }
}
