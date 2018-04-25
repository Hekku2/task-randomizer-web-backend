using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DataStorage.Interfaces;
using DataStorage.Implementations;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using System.IO;
using Backend.Hubs;
using Backend.Services;

namespace Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyMethod();
                        builder.AllowAnyHeader();
                    })
                );

            var mockStorage = new MockStorage();
            var mockGameSessionStorage = new MockGameSessionStorage();
            services.AddSingleton<IErrandStorage>((s) => mockStorage);
            services.AddSingleton<IGameStorage>((s) => mockStorage);
            services.AddSingleton<IGameErrandStorage>((s) => mockStorage);
            services.AddSingleton<IGameSessionStorage>((s) => mockGameSessionStorage);
            services.AddSingleton<IGameSessionEventStorage>((s) => mockGameSessionStorage);
            services.AddSingleton<IGameSessionErrandStorage>((s) => mockGameSessionStorage);
            services.AddTransient<IGameSessionService, GameSessionService>();
            services.AddMvc();

            services.AddSignalR();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Task Randomizer API", Version = "v1" });
                var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Randomizer API v1");
            });

            app.UseCors("AllowAllOrigins");
            app.UseSignalR(routes =>
            {
                routes.MapHub<GameSessionHub>("/gameSessionHub");
            });
            app.UseMvc();
        }
    }
}
