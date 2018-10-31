using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

using Swashbuckle.AspNetCore.Swagger;

namespace WebApplication1
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Cria o Banco de Dados para SqLite
            using (var client = new LogQuakeContext())
            {
                client.Database.EnsureCreated();
            }
        }
                
        // This method gets called by the runtime. Use this method to add services to the container..
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
            );

            services.AddMvc();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Log - Quake III Arena",
                    Description = "API para retornar informações das partidas realizadas no jogo Quake III Arena como por exemplo, quantidades de participantes, total de mortes e quantidades de mortes por jogador. " + Environment.NewLine + "Essas informações são obtidas através do log gerado pelo próprio jogo.",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Márcio de Souza Teixeira",
                        Email = "marcio79.teixeira@gmail.com",
                        Url = ""
                    },
                    License = new License
                    {
                        Name = "Use under LICX",
                        Url = "https://example.com/license"
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });


            services.AddDbContext<LogQuakeContext>(options =>
                //options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnectionString"), b => b.UseRowNumberForPaging()));
                options.UseSqlite(Configuration.GetConnectionString("SQLiteConnectionString")));

            services.AddScoped<ILogQuakeService, LogQuakeService>();
            services.AddScoped<IKillRepository, KillRepository>();

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

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

        }

    }
}
