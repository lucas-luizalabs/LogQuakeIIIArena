using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using System.IO;

namespace LogQuake.API
{
#pragma warning disable CS1591
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                              optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                    logging.AddEventLog(new EventLogSettings
                    {
                        SourceName = "Log Quake III Arena"
                    });
                    logging.SetMinimumLevel(LogLevel.Warning);
                    //Trace = 0
                    //Debug = 1
                    //Information = 2
                    //Warning = 3
                    //Error = 4
                    //Critical = 5
                })
                .UseStartup<Startup>()
                .Build();

            webHost.Run();
            //BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
#pragma warning disable CS1591
}
