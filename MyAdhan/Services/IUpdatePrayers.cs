using MyAdhan.Scheduler.Models;

namespace MyAdhan.Scheduler.Services
{
    public interface IUpdatePrayers
    {
        void Update(string json, IPrayers prayers);
    }
}