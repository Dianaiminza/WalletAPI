
using CoreApi.Infrastructure.Repository.Implementations;
using Infrastructure.Repository.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CoreApi.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services.AddScoped(typeof(IRepositoryUnit), typeof(RepositoryUnit));
  }

  public static void AddMappingProfile(this IServiceCollection services)
  {
    services.AddAutoMapper(Assembly.GetExecutingAssembly());
  }
}
