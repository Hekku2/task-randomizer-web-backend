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
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAllOrigins");
            app.UseMvc();
        }
    }
}
