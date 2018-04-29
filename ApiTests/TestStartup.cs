using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DataStorage.Interfaces;
using DataStorage.Implementations;
using Backend.Hubs;
using Backend.Services;

namespace ApiTests
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
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
            services.AddSingleton<IGameSessionService, GameSessionService>();
            services.AddMvc();

            services.AddSignalR(options => 
            {
                options.EnableDetailedErrors = true;
            });
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
            app.UseSignalR(routes =>
            {
                routes.MapHub<GameSessionHub>("/gameSessionHub");
            });
        }
    }
}
