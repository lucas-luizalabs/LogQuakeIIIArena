using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
//using AutoMapper;
using LogQuake.Domain.Entities;
using LogQuake.Domain.Interfaces;
using LogQuake.Infra.Data.Contexto;
using LogQuake.Infra.Data.Repositories;
using LogQuake.Service.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSwag.AspNetCore;

namespace WebApplication2
{
    /// <summary>
    /// Classe inicial do sistemma.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Construtor da classe recebendo Interface de Configuração.
        /// </summary>
        /// <param name="configuration">confguração a ser executado</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Método de configuração da classe Startup.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
            );

            //services.AddAutoMapper();

            services.AddMvc();

            services.AddSwagger();

            services.AddDbContext<LogQuakeContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LogQuakeDatabase"),b => b.UseRowNumberForPaging()));

            services.AddScoped<ILogQuakeService, LogQuakeService>();
            services.AddScoped<IKillRepository, KillRepository>();
            services.AddScoped(typeof(IServiceBase<>), typeof(ServiceBase<>));
            //IServiceBase<Kill> serviceBase
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseMvc();

            // Register the Swagger generator middleware
            app.UseSwaggerUi3WithApiExplorer(settings =>
            {
                settings.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Log ";
                    document.Info.Description = "API para retornar os logs";
                    document.Info.TermsOfService = "Nenhum termo de serviço";
                    document.Info.Contact = new NSwag.SwaggerContact
                    {
                        Name = "Márcio de Souza Teixeira",
                        Email = string.Empty,
                        Url = "http://marciodesouzateixeira.com"
                    };
                    document.Info.License = new NSwag.SwaggerLicense
                    {
                        Name = "Use under LICX",
                        Url = "https://example.com/license"
                    };
                };
            });

        }
    }
}
