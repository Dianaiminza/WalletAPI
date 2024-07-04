using Infrastructure.Shared.Services.Abstractions;
using Infrastructure.Shared.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Shared.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
  {
    services.AddTransient<ICurrentDateProvider, CurrentDateProvider>();
    services.AddTransient<ISymmetricEncryptionService, SymmetricEncryptionService>();
    return services;
  }
}
