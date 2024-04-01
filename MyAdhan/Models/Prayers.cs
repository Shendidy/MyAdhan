namespace MyAdhan.Scheduler.Models
{
    public class Prayers : IPrayers
    {
        public string Fajr { get; set; } = "0";
        public string Dhuhr { get; set; }
        public string Asr { get; set; }
        public string Maghrib { get; set; }
        public string Imsak { get; set; }
        public string Isha { get; set; }
        public DateOnly Date { get; set; }
        public string DateHijri { get; set; }
        public string RamadanQuran { get; set; }

        public IPrayers GetPrayers() => this;
    }
}
