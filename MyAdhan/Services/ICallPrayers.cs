using MyAdhan.Scheduler.Models;

namespace MyAdhan.Scheduler.Services
{
    public interface ICallPrayers
    {
        void CallEndpoints(IPrayers prayers);
    }
}