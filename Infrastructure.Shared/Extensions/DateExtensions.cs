namespace Infrastructure.Shared.Extensions;

public static class DateExtensions
{
  public static string DateTimeToIsoString(this DateTime dateTime)
  {
    //var dateString = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.FFF") + "Z";
    var dateString = dateTime.ToString("yyyy-MM-ddTHH:mm:ss");

    return dateString;
  }
  public static string ToIso8601String(this DateTimeOffset dateTimeOffset)
  {
    // var dateString = dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:ss.FFF") + "Z";
    var dateString = dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");

    return dateString;
  }
  public static string DateTimeOffsetToIsoString(this DateTimeOffset dateTimeOffset)
  {
    var dateString = dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:ss.FFF") + "Z";

    return dateString;
  }

  public static string ToDateWithoutTime(this DateTime dateTimeParam)
  {
    return dateTimeParam.ToEastAfricaTime().ToString("MM/dd/yyyy");
  }

  public static string ToDateWithTime(this DateTime dateTimeParam)
  {
    return dateTimeParam.ToEastAfricaTime().ToString("MM/dd/yyyy HH:mm:ss");
  }

  public static string ToDateWithTimeString(this DateTime dateTimeParam)
  {
    return dateTimeParam.ToString("MM/dd/yyyy HH:mm:ss");
  }

  public static string ToFormattedDateWithoutTime(this DateTime dateTimeParam)
  {
    return dateTimeParam.ToEastAfricaTime().ToString("MM/dd/yyyy");
  }

  public static string ToFormattedDateWithTime(this DateTime dateTimeParam)
  {
    return dateTimeParam.ToEastAfricaTime().ToString("MM/dd/yyyy HH:mm:ss");
  }

  public static string ToDashFormattedDateWithTime(this DateTime dateTimeParam)
  {
    return dateTimeParam.ToString("yyyy-MM-dd hh:mm:ss");
  }

  public static string ToDateWithoutTime(this DateTimeOffset dateTimeParam)
  {
    return dateTimeParam.ToEastAfricaTime().ToString("MM/dd/yyyy");
  }

  public static string ToDateWithTime(this DateTimeOffset dateTimeParam)
  {
    return dateTimeParam.ToEastAfricaTime().ToString("MM/dd/yyyy HH:mm:ss");
  }

  public static string ToDashFormattedDateWithTime(this DateTimeOffset dateTimeParam)
  {
    return dateTimeParam.ToString("yyyy-MM-dd hh:mm:ss");
  }

  public static DateTime ToEastAfricaTime(this DateTime dateTimeParam)
  {
    if (dateTimeParam.Kind == DateTimeKind.Unspecified)
    {
      dateTimeParam = DateTime.SpecifyKind(dateTimeParam.AddHours(-3), DateTimeKind.Utc);
    }

    var timeZone = GetTimeZoneInfo();
    return timeZone != null
      ? TimeZoneInfo.ConvertTime(dateTimeParam, timeZone)
      : dateTimeParam.ToLocalTime();
  }

  public static DateTime SetKind(this DateTime dT, DateTimeKind dTKind)
  {
    return DateTime.SpecifyKind(dT, dTKind);
  }

  public static DateTimeOffset ToEastAfricaTime(this DateTimeOffset dateTimeParam)
  {
    var timeZone = GetTimeZoneInfo();
    return timeZone != null
      ? TimeZoneInfo.ConvertTime(dateTimeParam, timeZone)
      : dateTimeParam.ToLocalTime();
  }

  private static TimeZoneInfo GetTimeZoneInfo()
  {
    var eAfricaStandardTime = "E. Africa Standard Time";
    TimeZoneInfo timeZone = null;
    try
    {
      timeZone = TimeZoneInfo.FindSystemTimeZoneById(eAfricaStandardTime);
    }
    catch (TimeZoneNotFoundException)
    {
      timeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(t => t.BaseUtcOffset == new TimeSpan(0, 3, 0, 0));
    }
    catch (InvalidTimeZoneException)
    {
      timeZone = null;
    }

    return timeZone;
  }
}
