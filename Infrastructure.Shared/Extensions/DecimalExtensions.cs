namespace Infrastructure.Shared.Extensions;

public static class DecimalExtensions
{
  public static decimal Round(this decimal value, int precision = 2)
  {
    return decimal.Round(value, precision, MidpointRounding.AwayFromZero);
  }

  public static bool IsInRangeInclusive(this decimal value, decimal min, decimal max)
  {
    return value >= min && value <= max;
  }
}
