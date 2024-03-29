using Microsoft.Extensions.Hosting;
using Coravel;
using Microsoft.Extensions.DependencyInjection;
using MyAdhan.Scheduler.Models;
using MyAdhan.Scheduler.Services;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Services.AddScheduler();
        builder.Services.AddTransient<GetAdhanTimings>();
        builder.Services.AddSingleton<IPrayers, Prayers>();
        builder.Services.AddTransient<IUpdatePrayers, UpdatePrayers>();
        builder.Services.AddTransient<IPrayer, Prayer>();
        builder.Services.AddTransient<ICallPrayers, CallPrayers>();

        var app = builder.Build();

        app.Services.UseScheduler(scheduler =>
        {
            scheduler.Schedule<GetAdhanTimings>()
            .EverySecond().Once();

            scheduler.Schedule<GetAdhanTimings>()
            .DailyAt(1,5); //using 01:05 to avoid confusion on 1st day of summer time savings change
        });

        app.Run();
    }
}