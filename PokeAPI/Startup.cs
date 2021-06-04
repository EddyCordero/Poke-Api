using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PokeAPI.Domain.Helpers;
using PokeAPI.Services;

namespace PokeAPI
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
            services.AddControllers();
                  //.AddNewtonsoftJson();

            AppSettings.BaseURL = Configuration[nameof(AppSettings.BaseURL)];
            AppSettings.ResultLimit = int.Parse(Configuration[nameof(AppSettings.ResultLimit)]);

            if (int.TryParse(Configuration[nameof(AppSettings.CacheExpirationInMinutes)],
               out var cacheExpirationInMinutes) &&
               cacheExpirationInMinutes < 30)
            {
                AppSettings.CacheExpirationInMinutes = cacheExpirationInMinutes;
            }

            services.AddScoped<IPokemonService, PokemonService>();
            services.AddMemoryCache();
            
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyHeader()
                    );

            });

           services.AddMvc().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } 

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
