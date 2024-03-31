using MyAdhan.Scheduler.Models;
using MyAdhan.Scheduler.Repositories;

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
                    Console.WriteLine($"Waiting for Fajr in {GetTimeLeft(fajr)}");
                    while (_myDate.GetNow() < fajr) { }
                }
                Console.WriteLine($"{_myDate.GetNow()} Calling for Fajr...");
                MakePrayerCall("VoiceMonkeyTriggers, Fajr");
            }

            // if now is <= Dhuhr
            if (_myDate.GetNow() <= dhuhr)
            {
                var msToDhuhr = Convert.ToInt32((dhuhr - _myDate.GetNow()).TotalMilliseconds);

                if (msToDhuhr > 1000)
                {
                    Console.WriteLine($"Waiting for Dhuhr in {GetTimeLeft(dhuhr)}");
                    while (_myDate.GetNow() < dhuhr) { }
                }
                Console.WriteLine($"{_myDate.GetNow()} Calling for Dhuhr...");
                MakePrayerCall("VoiceMonkeyTriggers, Dhuhr");
            }

            // if now is <= Asr
            if (_myDate.GetNow() <= asr)
            {
                var msToAsr = Convert.ToInt32((asr - _myDate.GetNow()).TotalMilliseconds);

                if (msToAsr > 1000)
                {
                    Console.WriteLine($"Waiting for Asr in {GetTimeLeft(asr)}");
                    while (_myDate.GetNow() < asr) { }
                }
                Console.WriteLine($"{_myDate.GetNow()} Calling for Asr...");
                MakePrayerCall("VoiceMonkeyTriggers, Asr");
            }

            // if now is <= Maghrib
            if (_myDate.GetNow() <= maghrib)
            {
                var msToMaghrib = Convert.ToInt32((maghrib - _myDate.GetNow()).TotalMilliseconds);

                if (msToMaghrib > 1000)
                {
                    Console.WriteLine($"Waiting for Maghrib in {GetTimeLeft(maghrib)}");
                    while (_myDate.GetNow() < maghrib) { }
                }
                Console.WriteLine($"{_myDate.GetNow()} Calling for Mahgrib...");
                MakePrayerCall("VoiceMonkeyTriggers, Maghrib");
            }

            // if now is <= Isha
            if (_myDate.GetNow() <= isha)
            {
                var msToIsha = Convert.ToInt32((isha - _myDate.GetNow()).TotalMilliseconds);

                if (msToIsha > 1000)
                {
                    Console.WriteLine($"Waiting for Isha in {GetTimeLeft(isha)}");
                    while (_myDate.GetNow() < isha) { }
                }
                Console.WriteLine($"{_myDate.GetNow()} Calling for Isha...");
                MakePrayerCall("VoiceMonkeyTriggers, Isha");
            }

            Console.WriteLine($"Called all prayers for today...");

            DateTime timeForGetNewTimings = today.ToDateTime(TimeOnly.Parse("1:05 AM")).AddDays(1);

            var waitingDuration = _isTesting ? 10000 : Convert.ToInt32(GetTimeLeft(_myDate.GetNow(), timeForGetNewTimings));
            Console.WriteLine($"Will get tomorrow's prayers times in: {TimeSpan.FromMilliseconds(waitingDuration)}");
            Thread.Sleep(waitingDuration);

            new GetAdhanTimings().GetTimings();
        }

        private void MakePrayerCall(string prayerUrl)
        {
            string muteTvUrl = string.Empty;

            if (_isTesting) prayerUrl = "VoiceMonkeyTriggers, Hi";
            else muteTvUrl = "VoiceMonkeyTriggers, MuteTvs";

            string[] calls = new string[] { muteTvUrl, prayerUrl };

            foreach (string call in calls)
            {
                using (var client = new HttpClient())
                {
                    var url = GetConfig(call);

                    if (!string.IsNullOrEmpty(url))
                    {
                        var uri = new Uri(url);

                        var result = client.GetAsync(uri).Result;
                        var json = result.Content.ReadAsStringAsync().Result;

                        Console.WriteLine($"Calling {call.Split(',').Last().Trim()} returned: {json}");
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
