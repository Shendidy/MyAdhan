using Coravel.Invocable;
using Microsoft.Extensions.Logging;

namespace MyAdhan.Scheduler.Services
{
    public class GetAdhanTimings : IInvocable
    {
        private readonly ILogger<GetAdhanTimings> _logger;
        private IUpdatePrayers _updatePrayers;

        public GetAdhanTimings(ILogger<GetAdhanTimings> logger, IUpdatePrayers updatePrayers)
        {
            _logger = logger;
            _updatePrayers = updatePrayers;
        }

        public Task Invoke()
        {
            _logger.LogInformation($"{DateTime.Now} - Getting new prayer times.");

            var baseUri = getConfig("PrayersApi, BaseUri");
            var endpoint = getConfig("PrayersApi, Endpoint");
            var dateToGet = DateTime.Now.ToString("dd-MM-yyyy");
            var paramAddress = getConfig("PrayersApi, ParamAddress");
            var paramMethod = getConfig("PrayersApi, ParamMethod");
            var paramTune = getConfig("PrayersApi, ParamTune");

            using (var client = new HttpClient())
            {
                var url = new Uri($"{baseUri}/{endpoint}/{dateToGet}?address={paramAddress}&method={paramMethod}&tune={paramTune}");

                var result = client.GetAsync(url).Result;
                var json = result.Content.ReadAsStringAsync().Result;

                _updatePrayers.Update(json);
            }

            return Task.FromResult(true);
        }

        private string getConfig(string path)
            => ConfigurationManager.GetConfigValue(path);
    }
}
