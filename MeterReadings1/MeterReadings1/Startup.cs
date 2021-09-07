using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeterReadings1.Models;
using MeterReadings1.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace MeterReadings1
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
            // Register instances of AccountRepository and MeterReadingRepository (Only 1 instance created)
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IMeterReadingRepository, MeterReadingRepository>();
            // Registering Context's for dependency injection
            services.AddDbContext<AccountContext>(o => o.UseSqlite("Data Source=accounts.db")); // Provided connectionstring to Sqlite db
            services.AddDbContext<MeterReadingContext>(o => o.UseSqlite("Data Source=meterReadings.db")); // Provided connectionstring to Sqlite db

            services.AddMvc();//New
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Version = "v1",
                    Title = "My API",
                    Description = "ASP.NET Core Web API MeterReadings"
                });
            });
            services.AddControllers();
            //services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseStaticFiles();//New

            app.UseRouting();

            app.UseAuthorization();

            // Enable middleware to serve Controllers
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Accounts}/{action=Index}/{id?}");
                //endpoints.MapControllers();
            });
        }
    }
}
