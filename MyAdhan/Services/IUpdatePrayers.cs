using MyAdhan.Scheduler.Models;
using MyAdhan.Scheduler.Repositories;

namespace MyAdhan.Scheduler.Services
{
    public interface IUpdatePrayers
    {
        void Update(string json, IPrayers prayers);
    }
}