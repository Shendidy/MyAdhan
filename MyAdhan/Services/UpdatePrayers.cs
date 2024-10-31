using MyAdhan.Scheduler.Enums;
using MyAdhan.Scheduler.Models;
using MyAdhan.Scheduler.Repositories;

using Newtonsoft.Json;

namespace MyAdhan.Scheduler.Services
{
    public class UpdatePrayers : IUpdatePrayers
    {
        private readonly IMyDate _myDate;
        public ICallPrayers _callPrayers;

        private readonly bool _isTesting = GetConfig("Testing") == "true";

        public UpdatePrayers() : this(
            new CallPrayers(),
            new MyDate()){}
        public UpdatePrayers(ICallPrayers callPrayers, IMyDate myDate)
        {
            _callPrayers = callPrayers;
            _myDate = myDate;
        }

        public void Update(string json, IPrayers prayers)
        {
            if (prayers == null)
            {
                Console.WriteLine($"No prayers object is created yet!");
            }
            else
            {
                Console.WriteLine($"Updating prayers timings...");

                dynamic details = JsonConvert.DeserializeObject<dynamic>(json);

                int hijriMonthNumber = Int32.Parse(details.SelectToken(GetConfig("PrayersApiResponse, HijriMonthNumber")).ToString());
                string hijriMonth = ((ArabicMonths)hijriMonthNumber).ToString();
                string hijriYear = details.SelectToken(GetConfig("PrayersApiResponse, HijriYear"));
                string hijriDay = details.SelectToken(GetConfig("PrayersApiResponse, HijriDay"));

                prayers.DateHijri = $"{hijriDay} {hijriMonth} {hijriYear}";

                if (_isTesting)
                {
                    prayers.Imsak = _myDate.GetNow().AddSeconds(5).ToString("HH:mm:ss");
                    prayers.Fajr = _myDate.GetNow().AddSeconds(10).ToString("HH:mm:ss");
                    prayers.Dhuhr = _myDate.GetNow().AddSeconds(15).ToString("HH:mm:ss");
                    prayers.Asr = _myDate.GetNow().AddSeconds(20).ToString("HH:mm:ss");
                    prayers.RamadanQuran = _myDate.GetNow().AddSeconds(25).ToString("HH:mm:ss");
                    prayers.Maghrib = _myDate.GetNow().AddSeconds(30).ToString("HH:mm:ss");
                    prayers.Isha = _myDate.GetNow().AddSeconds(35).ToString("HH:mm:ss");
                }
                else
                {
                    prayers.Fajr = details.SelectToken(GetConfig("PrayersApiResponse, Fajr"));
                    prayers.Imsak = TimeOnly.FromTimeSpan(TimeSpan.Parse(prayers.Fajr)).AddMinutes(-10).ToString();
                    prayers.Dhuhr = details.SelectToken(GetConfig("PrayersApiResponse, Dhuhr"));
                    prayers.Asr = details.SelectToken(GetConfig("PrayersApiResponse, Asr"));
                    prayers.Maghrib = details.SelectToken(GetConfig("PrayersApiResponse, Maghrib"));
                    prayers.RamadanQuran = TimeOnly.FromTimeSpan(TimeSpan.Parse(prayers.Maghrib)).AddMinutes(-10).ToString();
                    prayers.Isha = details.SelectToken(GetConfig("PrayersApiResponse, Isha"));
                }

                var datePath = GetConfig("PrayersApiResponse, Date");
                var dateFormat = GetConfig("PrayersApiResponse, DateFormat");
                var date = details.SelectToken(datePath).ToString();
                prayers.Date = DateOnly.ParseExact(date, dateFormat);

                LogPrayerTimes(prayers);

                _callPrayers.CallEndpoints(prayers);
            }
        }

        private void LogPrayerTimes(IPrayers prayers)
        {
            int width = 29;
            if (_isTesting) Console.WriteLine($"Test values...");

            Console.WriteLine($"  {String.Concat(Enumerable.Repeat("*", width))}");
            Console.WriteLine($"  {GetLog(GetConfig("PrayersApi, ParamAddress").Replace(",", ", "), width - 2)}");
            Console.WriteLine($"  {GetLog(prayers.Date.DayOfWeek.ToString(), width-2)}");
            Console.WriteLine($"  {GetLog(prayers.Date.ToString("dd MMMM yyyy"), width-2)}");
            Console.WriteLine($"  {GetLog(prayers.DateHijri, width-2)}");
            Console.WriteLine($"  {String.Concat(Enumerable.Repeat("*", width))}");
            Console.WriteLine($"  * Fajr:     {prayers.Fajr}           *");
            Console.WriteLine($"  * Dhuhr:    {prayers.Dhuhr}           *");
            Console.WriteLine($"  * Asr:      {prayers.Asr}           *");
            Console.WriteLine($"  * Maghrib:  {prayers.Maghrib}           *");
            Console.WriteLine($"  * Isha:     {prayers.Isha}           *");
            Console.WriteLine($"  {String.Concat(Enumerable.Repeat("*", width))}");
        }

        private string GetLog(string log, int width)
        {
            int logLength = log.Length;
            int leftGap = Convert.ToInt32(Math.Floor((width - logLength)/2m));
            int rightGap = width - logLength - leftGap;

            string left = String.Concat(Enumerable.Repeat(" ", leftGap));
            string right = String.Concat(Enumerable.Repeat(" ", rightGap));


            return $"*{left}{log}{right}*";
        }

        private static string GetConfig(string path)
            => ConfigurationManager.GetConfigValue(path);
    }
}
