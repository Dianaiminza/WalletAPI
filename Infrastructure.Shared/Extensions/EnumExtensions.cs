using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Shared.Extensions;

public static class EnumExtensions
{
  public static string GetDescription(this Enum enumValue)
  {
    var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

    var descriptionAttributes =
      (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

    return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
  }

  public static string GetDisplayName(this Enum enumValue)
  {
    var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

    var displayAttributes =
      (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

    return displayAttributes.Length > 0 ? displayAttributes[0].Name : enumValue.ToString();
  }

  public static HashSet<T> GetEnumMatchingValues<T>(this string searchTerm) where T : Enum
  {
    var result = new HashSet<T>();
    if (string.IsNullOrEmpty(searchTerm))
    {
      return result;
    }

    searchTerm = searchTerm.ToLowerInvariant();
    foreach (Enum enumItem in Enum.GetValues(typeof(T)))
    {
      if (nameof(enumItem).ToLowerInvariant().Contains(searchTerm) ||
          enumItem.GetDescription().ToLowerInvariant().Contains(searchTerm))
      {
        result.Add((T)enumItem);
      }
    }

    return result;
  }

  public static IList<string> GetEnumKeys<T>() where T : Enum
  {
    return Enum.GetValues(typeof(T)).Cast<T>().Select(e => e.ToString()).ToList();
  }

  public static IList<T> GetEnumList<T>() where T : Enum
  {
    return Enum.GetValues(typeof(T)).Cast<T>().ToList();
  }
}
