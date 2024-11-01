﻿using System.Runtime.CompilerServices;

namespace MyAdhan.Scheduler.Models
{
    public interface IPrayers
    {
        string Asr { get; set; }
        DateOnly Date { get; set; }
        string Dhuhr { get; set; }
        string Fajr { get; set; }
        string DateHijri { get; set; }
        string Imsak { get; set; }
        string Isha { get; set; }
        string Maghrib { get; set; }
        string RamadanQuran { get; set; }

        IPrayers GetPrayers();
    }
}