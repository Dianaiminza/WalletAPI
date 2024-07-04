using Application.Assembly;
using CoreApi.Infrastructure.Extensions;
using Domain.Configuration;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Shared.Extensions;
using Infrastructure.Shared.Services.HttpClients;
using MediatR;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using WalletAPI.Extensions;
using WalletAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);
builder.Logging.ClearProviders();
builder.Services.AddControllers();
builder.Services.AddSwaggerConfiguration();
builder.Services
       .AddCorsConfiguration()
       .AddOptionConfigurations(builder.Configuration)
       .AddSharedInfrastructure()
       .AddRepositories()
       .AddDatabaseContext(builder.Configuration);

builder.Services.AddSharedServices();
builder.Services.AddLazyCache();
builder.Services.AddTransient<IRestClient, JsonRestClient>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
if (bool.TryParse(builder.Configuration[$"{RuntimeSystemConfiguration.SectionName}:{RuntimeSystemConfiguration.ForceHttpsRedirect}"],
      out var forceHttpsRedirect))
{
    if (forceHttpsRedirect)
    {
        app.UseHttpsRedirection();
    }
}

// set base path just like concept of virtual directory
var pathPrefix =
  app.Configuration[$"{RuntimeSystemConfiguration.SectionName}:{RuntimeSystemConfiguration.PathPrefix}"] ?? string.Empty;

if (!string.IsNullOrEmpty(pathPrefix) && pathPrefix.StartsWith("/"))
{
    app.UsePathBase(pathPrefix);
}

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"{pathPrefix}/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
    }
});

app.UseCors("CorsPolicy");
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.MigrateDatabase();



app.Run();
