using MyAdhan.Scheduler.Models;
using MyAdhan.Scheduler.Repositories;

namespace MyAdhan.Scheduler.Services
{
    public class GetAdhanTimings
    {
        private readonly IUpdatePrayers _updatePrayers;
        private readonly IMyDate _myDate;
        public IPrayers _prayers;

        public GetAdhanTimings() : this(
            new UpdatePrayers(),
            new Prayers(),
            new MyDate()) { }

        public GetAdhanTimings( IUpdatePrayers updatePrayers,
                                IPrayers prayers,
                                IMyDate myDate)
        {
            _updatePrayers = updatePrayers;
            _prayers = prayers;
            _myDate = myDate;
        }

        public void GetTimings()
        {
            var baseUri = GetConfig("PrayersApi, BaseUri");
            var endpoint = GetConfig("PrayersApi, Endpoint");
            var dateToGet = _myDate.GetNow().ToString("dd-MM-yyyy");
            var paramAddress = GetConfig("PrayersApi, ParamAddress");
            var paramMethod = GetConfig("PrayersApi, ParamMethod");
            var paramTune = GetConfig("PrayersApi, ParamTune");

            Console.WriteLine($"{_myDate.GetNow()} - Getting new prayer times...");

            using (var client = new HttpClient())
            {
                var url = new Uri($"{baseUri}/{endpoint}/{dateToGet}?address={paramAddress}&method={paramMethod}&tune={paramTune}");

                var result = client.GetAsync(url).Result;
                var json = result.Content.ReadAsStringAsync().Result;

                _updatePrayers.Update(json, _prayers);
            }
        }

        private static string GetConfig(string path)
            => ConfigurationManager.GetConfigValue(path);
    }
}
