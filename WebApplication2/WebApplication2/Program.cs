using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApplication2
{
    /// <summary>
    /// Classe inicial do sistema
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Método inicial do sistema
        /// </summary>
        /// /// <param name="args">argumentos para ser iniciados o programa</param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        /// <summary>
        /// Método para construir o Web Host
        /// </summary>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
