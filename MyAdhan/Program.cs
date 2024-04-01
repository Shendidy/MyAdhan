using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using MyAdhan.Scheduler.Models;
using MyAdhan.Scheduler.Services;
using MyAdhan.Scheduler.Repositories;

public class Program
{
    private static void Main(string[] args)
    {
        var host = AppStartup();

        var service = ActivatorUtilities.CreateInstance<GetAdhanTimings>(host.Services);

        service.GetTimings();
    }

    static IHost AppStartup()
    {
        var builder = new ConfigurationBuilder();

        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(builder.Build())
                        .WriteTo.Console()
                        .CreateLogger();

        Log.Logger.Information("starting serilog in a console app...");

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) => {
                services.AddSingleton<IConfigurationManager, ConfigurationManager>();
                services.AddTransient<GetAdhanTimings>();
                services.AddTransient<IPrayers, Prayers>();
                services.AddTransient<IUpdatePrayers, UpdatePrayers>();
                services.AddTransient<ICallPrayers, CallPrayers>();
                services.AddTransient<IMyDate, MyDate>();
            })
            .UseSerilog()
            .Build();

        return host;
    }
}