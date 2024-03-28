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
            _updatePrayers.Update();

            return Task.FromResult(true);
        }
    }
}
