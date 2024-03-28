namespace MyAdhan.Scheduler.Models
{
    public interface IPrayer
    {
        int Hour { get; set; }
        int Minute { get; set; }
        string Name { get; set; }
    }
}