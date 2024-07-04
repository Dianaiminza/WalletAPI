namespace Infrastructure.Shared.Services.Abstractions;

public interface ICurrentDateProvider
{
  DateTimeOffset Now { get; }
  DateTimeOffset NowUtc { get; }
}
