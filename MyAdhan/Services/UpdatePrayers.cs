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
        public UpdatePrayers(ILogger<UpdatePrayers> logger, IPrayers prayers, ICallPrayers callPrayers)
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
                //string jsonFilePath = @".\JsonFiles\AdhanAPIResponse.json";
                //string json = File.ReadAllText(jsonFilePath);

                dynamic details = JsonConvert.DeserializeObject<dynamic>(json);

                int hijriMonthNumber = Int32.Parse(details.SelectToken("data.date.hijri.month.number").ToString());
                string hijriMonth = ((ArabicMonths)hijriMonthNumber).ToString();
                string hijriYear = details.SelectToken("data.date.hijri.year").ToString();
                string hijriDay = details.SelectToken("data.date.hijri.day").ToString();

                _prayers.Fajr = details.SelectToken("data.timings.Fajr").ToString();
                _prayers.Dhuhr = details.SelectToken("data.timings.Dhuhr").ToString();
                _prayers.Asr = details.SelectToken("data.timings.Asr").ToString();
                _prayers.Maghrib = details.SelectToken("data.timings.Maghrib").ToString();
                _prayers.Isha = details.SelectToken("data.timings.Isha").ToString();
                _prayers.DateHijri = $"{hijriDay} {hijriMonth} {hijriYear}";
                _prayers.Date = DateOnly.ParseExact(
                    details.SelectToken("data.date.gregorian.date").ToString()
                    , "dd-MM-yyyy");

                _callPrayers.CallEndpoints();
            }
        }
    }
}
