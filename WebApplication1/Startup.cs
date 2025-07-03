using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Repositories;
using Microsoft.AspNetCore.Http;
using WebApplication1.Services;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Provides access to appsettings.json and environment variables
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime to add services to the DI container
        public void ConfigureServices(IServiceCollection services)
        {
            // ===========================================
            // JWT Authentication Configuration
            // ===========================================
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Validate the token issuer (who created the token)
                        ValidateIssuer = true,

                        // Validate the audience (who the token is intended for)
                        ValidateAudience = true,

                        // Ensure the token hasn't expired
                        ValidateLifetime = true,

                        // Ensure the token signature is valid
                        ValidateIssuerSigningKey = true,

                        // The expected issuer value (from appsettings.json)
                        ValidIssuer = Configuration["Jwt:Issuer"],

                        // The expected audience value (from appsettings.json)
                        ValidAudience = Configuration["Jwt:Audience"],

                        // The secret key used to sign tokens
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])
                        )
                    };
                });

            // ===========================================
            // Enable CORS (Cross-Origin Resource Sharing)
            // ===========================================
            services.AddCors(c =>
                c.AddPolicy("AllowOrigin", options =>
                    options
                        .AllowAnyOrigin()    // Allow any domain
                        .AllowAnyMethod()    // Allow GET, POST, PUT, DELETE, etc.
                        .AllowAnyHeader()    // Allow any HTTP headers
                )
            );

            // ===========================================
            // Register EF Core DbContext with MySQL
            // ===========================================
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("TaskManagementApp"),
                    ServerVersion.AutoDetect(Configuration.GetConnectionString("TaskManagementApp"))
                )
            );

            // ===========================================
            // Register Repositories for Dependency Injection
            // ===========================================
            AddDIForRepos(services);

            // ===========================================
            // Register Services for Dependency Injection
            // ===========================================
            AddDIForServices(services);

            //Add context so that repository can have dependency injection
            services.AddHttpContextAccessor();

            // ===========================================
            // Register Controllers and Configure JSON Serialization
            // ===========================================
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                    options.SerializerSettings.NullValueHandling =
                        Newtonsoft.Json.NullValueHandling.Ignore;

                    options.SerializerSettings.Formatting =
                        Newtonsoft.Json.Formatting.Indented;
                });

            // ===========================================
            // Register Swagger for API Documentation
            // ===========================================
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebApplication1",
                    Version = "v1"
                });

                // Define Bearer token in Swagger UI
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.
                                    Enter 'Bearer' [space] and then your token in the text box below.
                                    Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                // Make Swagger require the Bearer token for secured endpoints
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        // Helper method to register Repositories
        public void AddDIForRepos(IServiceCollection services)
        {
            services.AddScoped<TaskRepository>();
            services.AddScoped<UserRepository>();
        }

        // Helper method to register Services
        public void AddDIForServices(IServiceCollection services)               
        {
            services.AddScoped<TaskService>();
            services.AddScoped<UserService>();
        }

        // This method configures the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // ===========================================
            // Run EF Core migrations automatically
            // ===========================================
            ApplyMigrations(app);

            // Enable CORS middleware
            app.UseCors(options =>
                options
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );

            if (env.IsDevelopment())
            {
                // Show detailed error pages in development
                app.UseDeveloperExceptionPage();

                // Enable Swagger UI for testing APIs
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1")
                );
            }

            // Enable routing
            app.UseRouting();

            // Enable authentication middleware (checks JWT tokens)
            app.UseAuthentication();

            // Enable authorization middleware (enforces role-based policies)
            app.UseAuthorization();

            // Map controller endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Run migrations on application startup.
        /// - EnsureCreated creates the database if it doesn't exist.
        /// - Migrate applies migrations if any exist.
        /// </summary>
        private void ApplyMigrations(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

            try
            {
                // Ensure database exists
                context.Database.EnsureCreated();

                // Apply migrations (if any)
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration error: {ex.Message}");
                // Optionally rethrow or handle as needed
            }
        }
    }
}
