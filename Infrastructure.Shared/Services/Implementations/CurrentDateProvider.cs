using Infrastructure.Shared.Services.Abstractions;

namespace Infrastructure.Shared.Services.Implementations;

public class CurrentDateProvider : ICurrentDateProvider
{
  public DateTimeOffset Now => DateTimeOffset.Now;
  public DateTimeOffset NowUtc => DateTimeOffset.UtcNow;
}
