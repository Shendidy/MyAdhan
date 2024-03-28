//using Coravel;

//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using MyAdhan.Scheduler.Services;
//using MyAdhan.Scheduling.ConsoleApp;

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace MyAdhan.Scheduler
//{
//    public static class Test
//    {
//        public static void testtime(IServiceCollection services)
//        {
//            var host = CreateHostBuilder().Build();
//            host.Services.UseScheduler(scheduler =>
//            {
//                var dailyUpdateSchedule = scheduler.ScheduleWithParams<CallPrayers>("from test");
//                dailyUpdateSchedule.EverySeconds(2);
//            });

//            host.Run();
//        }

//        private static IHostBuilder CreateHostBuilder() =>
//      Host.CreateDefaultBuilder()
//          .ConfigureServices((hostContext, services) =>
//          {
//              services.AddScheduler();
//          });
//    }
//}
