using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Pomelo.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Repositories;
using WebApplication1.Services;

namespace WebApplication1
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
            // Enable CORS
            services.AddCors(c =>
                c.AddPolicy("AllowOrigin", options =>
                    options.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                )
            );

            // Add DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("TaskManagementApp"),
                    ServerVersion.AutoDetect(Configuration.GetConnectionString("TaskManagementApp"))
                )
            );

            // Add DI for repos
            AddDIForRepos(services);

            // Add DI for services
            AddDIForServices(services);

            // Add controllers + JSON settings
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                });

            // Add Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "WebApplication1",
                    Version = "v1"
                });
            });
        }

        public void AddDIForRepos(IServiceCollection services)
        {
            services.AddScoped<TaskRepository>();
        }

        public void AddDIForServices(IServiceCollection services)
        {
            services.AddScoped<TaskService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Apply migrations automatically (Option 1: Automatic)
            ApplyMigrations(app);

            //Enable CORS
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1"));
            }

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

       //This is migration function => use to create database and update tables if needed
        private void ApplyMigrations(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

            try
            {
                // Only creates database, doesn't apply migrations
                context.Database.EnsureCreated();

                // OR apply migrations (which also creates database)
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                // Log the error - you might want to inject ILogger here
                Console.WriteLine($"Migration error: {ex.Message}");
                // Optionally rethrow or handle as needed
            }
        }
    }

    
}