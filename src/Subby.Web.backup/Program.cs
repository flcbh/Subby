using Autofac.Extensions.DependencyInjection;
using LastContent.ServiceBus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Subby.Web.Messages;

namespace Subby.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

#if !DEBUG
            var serviceBus =
                (IServiceBusPublisher) host.Services.GetRequiredService(typeof(IServiceBusPublisher));
            
            // serviceBus.ScheduleRecurring("0 40 9 * * ?", new AdzunaMessage());
            // serviceBus.ScheduleRecurring("0 30 9 * * ?", new ReedMessage());
            // serviceBus.ScheduleRecurring("0 10 9 * * ?", new CvLibraryMessage());
            // serviceBus.ScheduleRecurring("0 */30 * ? * *", new ValidateAdsMessage());
            serviceBus.ScheduleRecurring("0 0 10 ? * MON", new NewJobMessage());
            // serviceBus.ScheduleRecurring("0 0 10 * * ?", new ValidateAdsMessage());
#endif

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseSerilog()
                        .UseStartup<Startup>()
                        .ConfigureLogging(logging =>
                        {
                            logging.ClearProviders();
                            logging.AddConsole();
                            // logging.AddAzureWebAppDiagnostics(); add this if deploying to Azure
                        });
                });
    }
}