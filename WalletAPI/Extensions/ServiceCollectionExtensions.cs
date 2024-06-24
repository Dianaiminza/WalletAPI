using CoreApi.Api.Swagger;
using Domain.Configuration;
using Domain.Shared;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace WalletAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddDatabaseContext(
      this IServiceCollection services,
      IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var enableLogging = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

            services.AddDbContext<DatabaseContext>(
                dbContextOptions => dbContextOptions
                    .UseSqlServer(connectionString, options =>
                    {
                        options.MigrationsAssembly("Infrastructure");
                    })
                    .EnableSensitiveDataLogging(enableLogging)
                    .EnableDetailedErrors(enableLogging));

            return services;
        }
        public static WebApplication MigrateDatabase(this WebApplication webApplication)
        {
            using var scope = webApplication.Services.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            if (appContext.Database.GetPendingMigrations().Any())
            {
                appContext.Database.Migrate();
            }

            return webApplication;
        }
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
        {
            return services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
        }
        internal static IServiceCollection AddOptionConfigurations(
      this IServiceCollection services,
      IConfiguration configuration)
        {
            services.Configure<JwtConfiguration>(configuration.GetSection(JwtConfiguration.SectionName));
            return services;
        }
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(o =>
            {
                o.GroupNameFormat = "'v'VVV";
                o.SubstituteApiVersionInUrl = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                        "Bearer",
                        new OpenApiSecurityScheme
                        {
                            Name = "Authorization",
                            Type = SecuritySchemeType.ApiKey,
                            Scheme = "Bearer",
                            BearerFormat = "JWT",
                            In = ParameterLocation.Header,
                            Description =
                                "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer token\""
                        });

                options.AddSecurityRequirement(
                        new OpenApiSecurityRequirement
                      {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] { }
                    }
                      });

                options.MapType<object>(() => new OpenApiSchema { Type = "object", Nullable = true });

                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"WalletApi.{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                var currentAssembly = Assembly.GetExecutingAssembly();
                var xmlDocs = currentAssembly
                        .GetReferencedAssemblies()
                        .Union(new[] { currentAssembly.GetName() })
                        .Select(a => Path.Combine(AppContext.BaseDirectory, $"{a.Name}.xml"))
                        .Where(File.Exists)
                        .ToArray();

                Array.ForEach(xmlDocs, d =>
                {
                    var doc = XDocument.Load(d);
                    options.IncludeXmlComments(() => new XPathDocument(doc.CreateReader()), true);
                });
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigurationOptions>();

            return services;
        }
        internal static IServiceCollection ConfigureJwtAuthentication(
      this IServiceCollection services,
      IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = configuration[$"{JwtConfiguration.SectionName}:Audience"],
                    ValidIssuer = configuration[$"{JwtConfiguration.SectionName}:Issuer"],
                    IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(configuration[$"{JwtConfiguration.SectionName}:Secret"]))
                };
                bearer.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = c =>
                    {
                        if (c.Exception is SecurityTokenExpiredException)
                        {
                            throw new ApiException("The token is expired", HttpStatusCode.Unauthorized);
                        }

                        throw new ApiException(
                                $"An unhandled error has occurred {c.Exception.Message}",
                                HttpStatusCode.Unauthorized);
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        if (!context.Response.HasStarted)
                        {
                            throw new ApiException("You are not authorized", HttpStatusCode.Unauthorized);
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = context => throw new ApiException(
                            "You are not authorized to access this resource",
                            HttpStatusCode.Forbidden),
                };
            });

            return services;
        }
    }
}
