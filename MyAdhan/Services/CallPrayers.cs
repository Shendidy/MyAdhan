using Microsoft.Extensions.Logging;
using MyAdhan.Scheduler.Models;

namespace MyAdhan.Scheduler.Services
{
    public class CallPrayers : ICallPrayers
    {
        private readonly ILogger<CallPrayers> _logger;
        public IPrayers _prayers;
        public CallPrayers(IPrayers prayers, ILogger<CallPrayers> logger)
        {
            _prayers = prayers;
            _logger = logger;
        }

        public async void CallEndpoints()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var fajr = today.ToDateTime(TimeOnly.Parse(_prayers.Fajr));
            var dhuhr = today.ToDateTime(TimeOnly.Parse(_prayers.Dhuhr));
            var asr = today.ToDateTime(TimeOnly.Parse(_prayers.Asr));
            var maghrib = today.ToDateTime(TimeOnly.Parse(_prayers.Maghrib));
            var isha = today.ToDateTime(TimeOnly.Parse(_prayers.Isha));

            // if now is <= Fajr
            // use delayers until Fajr, then call end point
            if (DateTime.Now <= fajr)
            {
                var msToFajr = Convert.ToInt32((fajr - DateTime.Now).TotalMilliseconds);

                if (msToFajr > 5000) await Task.Delay(msToFajr);
                _logger.LogInformation($"{DateTime.Now} Calling for Fajr...");
            }

            // if now is <= Dhuhr
            if (DateTime.Now <= dhuhr)
            {
                var msToDhuhr = Convert.ToInt32((dhuhr - DateTime.Now).TotalMilliseconds);

                if (msToDhuhr > 5000) await Task.Delay(msToDhuhr);
                _logger.LogInformation($"{DateTime.Now} Calling for Dhuhr...");
            }

            // if now is <= Asr
            if (DateTime.Now <= asr)
            {
                var msToAsr = Convert.ToInt32((asr - DateTime.Now).TotalMilliseconds);

                if (msToAsr > 5000) await Task.Delay(msToAsr);
                _logger.LogInformation($"{DateTime.Now} Calling for Asr...");
            }

            // if now is <= Maghrib
            if (DateTime.Now <= maghrib)
            {
                var msToMaghrib = Convert.ToInt32((maghrib - DateTime.Now).TotalMilliseconds);

                if (msToMaghrib > 5000) await Task.Delay(msToMaghrib);
                _logger.LogInformation($"{DateTime.Now} Calling for Mahgrib...");
            }

            // if now is <= Isha
            if (DateTime.Now <= isha)
            {
                var msToIsha = Convert.ToInt32((isha - DateTime.Now).TotalMilliseconds);

                if (msToIsha > 5000) await Task.Delay(msToIsha);
                _logger.LogInformation($"{DateTime.Now} Calling for Isha...");
            }

            _logger.LogInformation($"Called all prayers for today...");
        }
    }
}
