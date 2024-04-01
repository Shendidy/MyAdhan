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
            bool isRamadan = prayers.DateHijri.Contains("Ramadan");
            var today = _myDate.GetToday();
            var imsak = today.ToDateTime(TimeOnly.Parse(prayers.Imsak));
            var fajr = today.ToDateTime(TimeOnly.Parse(prayers.Fajr));
            var dhuhr = today.ToDateTime(TimeOnly.Parse(prayers.Dhuhr));
            var asr = today.ToDateTime(TimeOnly.Parse(prayers.Asr));
            var ramadanQuran = today.ToDateTime(TimeOnly.Parse(prayers.RamadanQuran));
            var maghrib = today.ToDateTime(TimeOnly.Parse(prayers.Maghrib));
            var isha = today.ToDateTime(TimeOnly.Parse(prayers.Isha));

            if (isRamadan && _myDate.GetNow() <= imsak) PrepareCall(imsak, "Imsak");
            if (_myDate.GetNow() <= fajr) PrepareCall(fajr, "Fajr");
            if (_myDate.GetNow() <= dhuhr) PrepareCall(dhuhr, "Dhuhr");
            if (_myDate.GetNow() <= asr)PrepareCall(asr, "Asr");
            if (isRamadan && _myDate.GetNow() <= ramadanQuran) PrepareCall(ramadanQuran, "RamadanQuran");
            if (_myDate.GetNow() <= maghrib)PrepareCall(maghrib, "Maghrib");
            if (_myDate.GetNow() <= isha) PrepareCall(isha, "Isha");

            Console.WriteLine($"Called all prayers for today...");

            DateTime timeToGettingNewTimings = today.ToDateTime(TimeOnly.Parse("1:05 AM")).AddDays(1);

            var waitingDuration = _isTesting ? 10000 : Convert.ToInt32(GetTimeLeft(_myDate.GetNow(), timeToGettingNewTimings).TotalMilliseconds);
            Console.WriteLine($"Will get tomorrow's prayers times in: {TimeSpan.FromMilliseconds(waitingDuration)}");
            Thread.Sleep(waitingDuration);

            new GetAdhanTimings().GetTimings();
        }

        private void PrepareCall(DateTime time, string itemToCall)
        {
            var msToTime = Convert.ToInt32((time - _myDate.GetNow()).TotalMilliseconds);

            if (msToTime > 1000)
            {
                Console.WriteLine($"Waiting for {itemToCall} in {GetTimeLeft(time)}");
                Thread.Sleep(msToTime);
            }
            Console.WriteLine($"{_myDate.GetNow()} Calling for {itemToCall}...");
            MakeCall($"VoiceMonkeyTriggers, {itemToCall}");
        }

        private void MakeCall(string prayerUrl)
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

        private static TimeSpan GetTimeLeft(DateTime startTime, DateTime endTime)
        {
            var timeLeft = endTime - startTime;
            return new TimeSpan(timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
        }
    }
}
