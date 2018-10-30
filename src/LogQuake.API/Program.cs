using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;


namespace WebApplication1
{
#pragma warning disable CS1591
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
#pragma warning disable CS1591
}
