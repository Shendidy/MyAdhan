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
            _logger.LogInformation($"{DateTime.Now} Doing something then saving the new adhan times to json file for now...");


            var baseUri = "https://api.aladhan.com";
            var endpoint = "timingsByAddress";
            var dateToGet = "28-03-2024";
            var paramAddress = "Romford,UK";
            var paramMethod = "15";
            var paramTune = "0,-2,0,2,2,2,0,2,0";

            using (var client = new HttpClient())
            {
                var url = new Uri($"{baseUri}/{endpoint}/{dateToGet}?address={paramAddress}&method={paramMethod}&tune={paramTune}");
                _logger.LogInformation(url.ToString());

                var result = client.GetAsync(url).Result;
                var json = result.Content.ReadAsStringAsync().Result;

                _updatePrayers.Update(json);
            }

            return Task.FromResult(true);
        }
    }
}
