using System.Runtime.CompilerServices;

namespace MyAdhan.Scheduler.Models
{
    public interface IPrayers
    {
        string Asr { get; set; }
        DateOnly Date { get; set; }
        string Dhuhr { get; set; }
        string Fajr { get; set; }
        string DateHijri { get; set; }
        string Isha { get; set; }
        string Maghrib { get; set; }

        IPrayers GetPrayers();
    }
}