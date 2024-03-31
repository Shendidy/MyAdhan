namespace MyAdhan.Scheduler.Repositories
{
    public interface IMyDate
    {
        DateTime GetNow();
        DateOnly GetToday();
    }
}