﻿using MyAdhan.Scheduler.Enums;
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
                    prayers.Fajr = _myDate.GetNow().AddSeconds(5).ToString("HH:mm:ss");
                    prayers.Dhuhr = _myDate.GetNow().AddSeconds(10).ToString("HH:mm:ss");
                    prayers.Asr = _myDate.GetNow().AddSeconds(15).ToString("HH:mm:ss");
                    prayers.Maghrib = _myDate.GetNow().AddSeconds(20).ToString("HH:mm:ss");
                    prayers.Isha = _myDate.GetNow().AddSeconds(25).ToString("HH:mm:ss");
                }
                else
                {
                    prayers.Fajr = details.SelectToken(GetConfig("PrayersApiResponse, Fajr"));
                    prayers.Dhuhr = details.SelectToken(GetConfig("PrayersApiResponse, Dhuhr"));
                    prayers.Asr = details.SelectToken(GetConfig("PrayersApiResponse, Asr"));
                    prayers.Maghrib = details.SelectToken(GetConfig("PrayersApiResponse, Maghrib"));
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
            if (_isTesting) Console.WriteLine($"Test values...");
            Console.WriteLine($"  {String.Concat(Enumerable.Repeat("*", 35))}");
            Console.WriteLine($"  {GetLog(prayers.Date.ToString("dd MMMM yyyy"))}");
            Console.WriteLine($"  {GetLog(prayers.DateHijri)}");
            Console.WriteLine($"  * Fajr:     {prayers.Fajr}                 *");
            Console.WriteLine($"  * Dhuhr:    {prayers.Dhuhr}                 *");
            Console.WriteLine($"  * Asr:      {prayers.Asr}                 *");
            Console.WriteLine($"  * Maghrib:  {prayers.Maghrib}                 *");
            Console.WriteLine($"  * Isha:     {prayers.Isha}                 *");
            Console.WriteLine($"  {String.Concat(Enumerable.Repeat("*", 35))}");
        }

        private string GetLog(string log)
        {
            int length = 33;

            int logLength = log.Length;
            int leftGap = Convert.ToInt32(Math.Floor((length - logLength)/2m));
            int rightGap = length - logLength - leftGap;

            string left = String.Concat(Enumerable.Repeat(" ", leftGap));
            string right = String.Concat(Enumerable.Repeat(" ", rightGap));


            return $"*{left}{log}{right}*";
        }

        private static string GetConfig(string path)
            => ConfigurationManager.GetConfigValue(path);
    }
}
