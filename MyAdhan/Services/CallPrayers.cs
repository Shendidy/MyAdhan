using MyAdhan.Scheduler.Models;

namespace MyAdhan.Scheduler.Services
{
    public class CallPrayers : ICallPrayers
    {
        private readonly bool _isTesting = GetConfig("Testing") == "true";

        public void CallEndpoints(IPrayers prayers)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var fajr = today.ToDateTime(TimeOnly.Parse(prayers.Fajr));
            var dhuhr = today.ToDateTime(TimeOnly.Parse(prayers.Dhuhr));
            var asr = today.ToDateTime(TimeOnly.Parse(prayers.Asr));
            var maghrib = today.ToDateTime(TimeOnly.Parse(prayers.Maghrib));
            var isha = today.ToDateTime(TimeOnly.Parse(prayers.Isha));

            // if now is <= Fajr
            if (DateTime.Now <= fajr)
            {
                var msToFajr = Convert.ToInt32((fajr - DateTime.Now).TotalMilliseconds);

                if (msToFajr > 1000)
                {
                    Console.WriteLine($"Waiting for Fajr in {TimeSpan.FromMilliseconds(msToFajr)}");
                    Thread.Sleep(msToFajr);
                }
                Console.WriteLine($"{DateTime.Now} Calling for Fajr...");
                MakePrayerCall(GetConfig("VoiceMonkeyTriggers, Fajr"));
            }

            // if now is <= Dhuhr
            if (DateTime.Now <= dhuhr)
            {
                var msToDhuhr = Convert.ToInt32((dhuhr - DateTime.Now).TotalMilliseconds);

                if (msToDhuhr > 1000)
                {
                    Console.WriteLine($"Waiting for Dhuhr in {TimeSpan.FromMilliseconds(msToDhuhr)}");
                    Thread.Sleep(msToDhuhr);
                }
                Console.WriteLine($"{DateTime.Now} Calling for Dhuhr...");
                MakePrayerCall(GetConfig("VoiceMonkeyTriggers, Dhuhr"));
            }

            // if now is <= Asr
            if (DateTime.Now <= asr)
            {
                var msToAsr = Convert.ToInt32((asr - DateTime.Now).TotalMilliseconds);

                if (msToAsr > 1000)
                {
                    Console.WriteLine($"Waiting for Asr in {TimeSpan.FromMilliseconds(msToAsr)}");
                    Thread.Sleep(msToAsr);
                }
                Console.WriteLine($"{DateTime.Now} Calling for Asr...");
                MakePrayerCall(GetConfig("VoiceMonkeyTriggers, Asr"));
            }

            // if now is <= Maghrib
            if (DateTime.Now <= maghrib)
            {
                var msToMaghrib = Convert.ToInt32((maghrib - DateTime.Now).TotalMilliseconds);

                if (msToMaghrib > 1000)
                {
                    Console.WriteLine($"Waiting for Maghrib in {TimeSpan.FromMilliseconds(msToMaghrib)}");
                    Thread.Sleep(msToMaghrib);
                }
                Console.WriteLine($"{DateTime.Now} Calling for Mahgrib...");
                MakePrayerCall(GetConfig("VoiceMonkeyTriggers, Maghrib"));
            }

            // if now is <= Isha
            if (DateTime.Now <= isha)
            {
                var msToIsha = Convert.ToInt32((isha - DateTime.Now).TotalMilliseconds);

                if (msToIsha > 1000)
                {
                    Console.WriteLine($"Waiting for Isha in {TimeSpan.FromMilliseconds(msToIsha)}");
                    Thread.Sleep(msToIsha);
                }
                Console.WriteLine($"{DateTime.Now} Calling for Isha...");
                MakePrayerCall(GetConfig("VoiceMonkeyTriggers, Isha"));
            }

            Console.WriteLine($"Called all prayers for today...");
            var timeTillGettingNewTimings =
                Convert.ToInt32((today.ToDateTime(TimeOnly.Parse("1:05 AM")).AddDays(1)
                - DateTime.Now).TotalMilliseconds);

            int hours = timeTillGettingNewTimings / 1000 / 60 / 60;
            int minutes = (timeTillGettingNewTimings - (hours * 60 * 60 * 1000)) / 1000 / 60;
            int seconds = (timeTillGettingNewTimings - (hours * 60 * 60 * 1000) - (minutes * 60 * 1000)) / 1000;

            Console.WriteLine($"{DateTime.Now} - Will get tomorrow's prayer times in: \n\t {hours} hours and {minutes} minutes, and {seconds} seconds.");
            Thread.Sleep(timeTillGettingNewTimings);
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
    }
}
