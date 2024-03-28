using Microsoft.Extensions.Hosting;
using Coravel;
using Microsoft.Extensions.DependencyInjection;
using MyAdhan.Scheduler.Models;
using MyAdhan.Scheduler.Services;

public class Program
{
    //private static void Main(string[] args)
    //{
    //    var host = CreateHostBuilder(args).Build();
    //    host.Services.UseScheduler(scheduler =>
    //    {
    //        var dailyUpdateSchedule = scheduler.Schedule<GetAdhanTimings>();
    //        dailyUpdateSchedule.EverySeconds(2).Once();
    //        //dailyUpdateSchedule.DailyAt(0,1);
    //    });

    //    host.Run();
    //}

    //private static IHostBuilder CreateHostBuilder(string[] args) =>
    //  Host.CreateDefaultBuilder(args)
    //      .ConfigureServices((hostContext, services) =>
    //      {
    //          services.AddScheduler();
    //          services.AddTransient<GetAdhanTimings>();
    //          services.AddSingleton<IPrayers, Prayers>();
    //          services.AddSingleton<IUpdatePrayers, UpdatePrayers>();
    //          services.AddTransient<IPrayer, Prayer>();
    //          services.AddTransient<ICallPrayers, CallPrayers>();
    //      });

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
            //.DailyAt(0,1);
        });

        app.Run();
    }
}