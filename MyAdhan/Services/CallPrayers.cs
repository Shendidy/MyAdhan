using MyAdhan.Scheduler.Models;
using MyAdhan.Scheduler.Repositories;

using System;

namespace MyAdhan.Scheduler.Services
{
    public class CallPrayers : ICallPrayers
    {
        private readonly bool _isTesting = GetConfig("Testing") == "true";
        private readonly IMyDate _myDate;

        public CallPrayers() : this(new MyDate()) { }

        public CallPrayers(IMyDate myDate)
        {
            _myDate = myDate;
        }

        public void CallEndpoints(IPrayers prayers)
        {
            var today = _myDate.GetToday();
            var fajr = today.ToDateTime(TimeOnly.Parse(prayers.Fajr));
            var dhuhr = today.ToDateTime(TimeOnly.Parse(prayers.Dhuhr));
            var asr = today.ToDateTime(TimeOnly.Parse(prayers.Asr));
            var maghrib = today.ToDateTime(TimeOnly.Parse(prayers.Maghrib));
            var isha = today.ToDateTime(TimeOnly.Parse(prayers.Isha));

            // if now is <= Fajr
            if (_myDate.GetNow() <= fajr)
            {
                var msToFajr = Convert.ToInt32((fajr - _myDate.GetNow()).TotalMilliseconds);

                if (msToFajr > 1000)
                {
                    while (_myDate.GetNow() < fajr)
                    {
                        Console.Write($"\r Waiting for Fajr in {GetTimeLeft(fajr)}");
                    }
                }
                Console.WriteLine($"{_myDate.GetNow()} Calling for Fajr...");
                MakePrayerCall(GetConfig("VoiceMonkeyTriggers, Fajr"));
            }

            // if now is <= Dhuhr
            if (_myDate.GetNow() <= dhuhr)
            {
                var msToDhuhr = Convert.ToInt32((dhuhr - _myDate.GetNow()).TotalMilliseconds);

                if (msToDhuhr > 1000)
                {
                    while (_myDate.GetNow() < dhuhr)
                    {
                        Console.Write($"\r Waiting for Dhuhr in {GetTimeLeft(dhuhr)}");
                    }
                }
                Console.WriteLine($"{_myDate.GetNow()} Calling for Dhuhr...");
                MakePrayerCall(GetConfig("VoiceMonkeyTriggers, Dhuhr"));
            }

            // if now is <= Asr
            if (_myDate.GetNow() <= asr)
            {
                var msToAsr = Convert.ToInt32((asr - _myDate.GetNow()).TotalMilliseconds);

                if (msToAsr > 1000)
                {
                    while (_myDate.GetNow() < asr)
                    {
                        Console.Write($"\r Waiting for Asr in {GetTimeLeft(asr)}");
                    }
                }
                Console.WriteLine($"{_myDate.GetNow()} Calling for Asr...");
                MakePrayerCall(GetConfig("VoiceMonkeyTriggers, Asr"));
            }

            // if now is <= Maghrib
            if (_myDate.GetNow() <= maghrib)
            {
                var msToMaghrib = Convert.ToInt32((maghrib - _myDate.GetNow()).TotalMilliseconds);

                if (msToMaghrib > 1000)
                {
                    while (_myDate.GetNow() < maghrib)
                    {
                        Console.Write($"\r Waiting for Maghrib in {GetTimeLeft(maghrib)}");
                    }
                }
                Console.WriteLine($"{_myDate.GetNow()} Calling for Mahgrib...");
                MakePrayerCall(GetConfig("VoiceMonkeyTriggers, Maghrib"));
            }

            // if now is <= Isha
            if (_myDate.GetNow() <= isha)
            {
                var msToIsha = Convert.ToInt32((isha - _myDate.GetNow()).TotalMilliseconds);

                if (msToIsha > 1000)
                {
                    while (_myDate.GetNow() < isha)
                    {
                        Console.Write($"\r Waiting for Isha in {GetTimeLeft(isha)}");
                    }
                }
                Console.WriteLine($"{_myDate.GetNow()} Calling for Isha...");
                MakePrayerCall(GetConfig("VoiceMonkeyTriggers, Isha"));
            }

            Console.WriteLine($"Called all prayers for today...");

            DateTime timeForGetNewTimings = today.ToDateTime(TimeOnly.Parse("1:05 AM")).AddDays(1);

            while (_myDate.GetNow() < timeForGetNewTimings)
            {
                Console.Write($"\rWill get tomorrow's prayers times in: {GetTimeLeft(_myDate.GetNow(), timeForGetNewTimings)}");
            }
            new GetAdhanTimings().GetTimings();
        }

        private void MakePrayerCall(string prayerUrl)
        {
            string muteTvUrl = string.Empty;

            if (_isTesting) prayerUrl = GetConfig("VoiceMonkeyTriggers, Hi");
            else muteTvUrl = GetConfig("VoiceMonkeyTriggers, MuteTvs");
            string[] calls = new string[] { muteTvUrl, prayerUrl };

            foreach (string call in calls)
            {
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(call))
                    {
                        var url = new Uri(call);

                        var result = client.GetAsync(url).Result;
                        var json = result.Content.ReadAsStringAsync().Result;

                        Console.WriteLine(json);
                    }
                }
            }
        }

        private static string GetConfig(string path)
            => ConfigurationManager.GetConfigValue(path);

        private TimeSpan GetTimeLeft(DateTime time)
        {
            var timeLeft = time - _myDate.GetNow();
            return new TimeSpan(timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
        }

        private TimeSpan GetTimeLeft(DateTime startTime, DateTime endTime)
        {
            var timeLeft = endTime - startTime;
            return new TimeSpan(timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
        }
    }
}
