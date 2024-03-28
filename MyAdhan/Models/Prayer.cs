namespace MyAdhan.Scheduler.Models
{
    public class Prayer : IPrayer
    {
        public string Name { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}
