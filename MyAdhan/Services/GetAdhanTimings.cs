using MyAdhan.Scheduler.Models;

namespace MyAdhan.Scheduler.Services
{
    public class GetAdhanTimings
    {
        private readonly IUpdatePrayers _updatePrayers;
        public IPrayers _prayers;

        public GetAdhanTimings() : this(
            new UpdatePrayers(),
            new Prayers()) { }

        public GetAdhanTimings( IUpdatePrayers updatePrayers,
                                IPrayers prayers)
        {
            _updatePrayers = updatePrayers;
            _prayers = prayers;
        }

        public void GetTimings()
        {
            Console.WriteLine($"{DateTime.Now} - Getting new prayer times.");

            var baseUri = GetConfig("PrayersApi, BaseUri");
            var endpoint = GetConfig("PrayersApi, Endpoint");
            var dateToGet = DateTime.Now.ToString("dd-MM-yyyy");
            var paramAddress = GetConfig("PrayersApi, ParamAddress");
            var paramMethod = GetConfig("PrayersApi, ParamMethod");
            var paramTune = GetConfig("PrayersApi, ParamTune");

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
