using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Infrastructure.Shared.Serializers;

public static class SerializerSettingsHelper
{
  public static JsonSerializerSettings CamelCase()
  {
    return new JsonSerializerSettings
    {
      ContractResolver = new DefaultContractResolver
      {
        NamingStrategy = new CamelCaseNamingStrategy()
      }
    };
  }

  public static JsonSerializerSettings SnakeCase()
  {
    return new JsonSerializerSettings
    {
      ContractResolver = new DefaultContractResolver
      {
        NamingStrategy = new SnakeCaseNamingStrategy()
      }
    };
  }
}
