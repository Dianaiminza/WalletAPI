using CoreApi.Api.Swagger;
using Domain.Configuration;
using Infrastructure.Context;
using Infrastructure.Shared.CustomExceptions;
using Infrastructure.Shared.Services.Abstractions;
using Infrastructure.Shared.Services.Implementations;
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
using WalletAPI.Swagger;

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
        internal static void AddSharedServices(this IServiceCollection services)
        {
            services.AddScoped<IExcelService, ExcelService>();
        }
        internal static IServiceCollection AddOptionConfigurations(
      this IServiceCollection services,
      IConfiguration configuration)
        {
            services.Configure<JwtConfiguration>(configuration.GetSection(JwtConfiguration.SectionName));
            return services;
        }
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
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

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Wallet Api Key",
                    Name = "x-api-key",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme, Id = "ApiKey"
                        }
                    },
                    new List<string>()
                }
        });

                c.MapType<object>(() => new OpenApiSchema { Type = "object", Nullable = true });

                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                // Set the comments path for the Swagger JSON and UI.
                var currentAssembly = Assembly.GetExecutingAssembly();
                var xmlDocs = currentAssembly.GetReferencedAssemblies()
                  .Union(new[] { currentAssembly.GetName() })
                  .Select(a => Path.Combine(AppContext.BaseDirectory, $"{a.Name}.xml"))
                  .Where(File.Exists).ToArray();

                Array.ForEach(xmlDocs, d =>
                {
                    var doc = XDocument.Load(d);
                    c.IncludeXmlComments(() => new XPathDocument(doc.CreateReader()), true);
                    c.SchemaFilter<SwaggerDescribeEnumMembers>(doc);
                });
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigurationOptions>();
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
