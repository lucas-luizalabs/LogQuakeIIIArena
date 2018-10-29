using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

namespace WebApplication1
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
                
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
            );

            services.AddMvc();

            services.AddSwagger();

            services.AddDbContext<LogQuakeContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LogQuakeDatabase"), b => b.UseRowNumberForPaging()));

            services.AddScoped<ILogQuakeService, LogQuakeService>();
            services.AddScoped<IKillRepository, KillRepository>();
            services.AddScoped(typeof(IServiceBase<>), typeof(ServiceBase<>));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                    document.Info.Title = "Log - Quake III Arena";
                    document.Info.Description = "API para retornar informações das partidas realizadas no jogo Quake III Arena como por exemplo, quantidades de participantes, total de mortes e quantidades de mortes por jogador. " + Environment.NewLine + "Essas informações são obtidas através do log gerado pelo próprio jogo.";
                    document.Info.TermsOfService = "Nenhum termo de serviço";
                    document.Info.Contact = new NSwag.SwaggerContact
                    {
                        Name = "Márcio de Souza Teixeira",
                        Email = "marcio79.teixeira@gamil.com",
                        Url = ""
                    };
                    //document.Info.License = new NSwag.SwaggerLicense
                    //{
                    //    Name = "Use under LICX",
                    //    Url = "https://example.com/license"
                    //};
                };
            });

        }

    }
}
