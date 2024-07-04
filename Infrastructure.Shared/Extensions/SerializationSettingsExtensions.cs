using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Infrastructure.Shared.Extensions;

public static class SerializationSettingsExtensions
{
  public static JsonSerializerSettings GetSettings(NamingStrategy namingStrategy = null)
  {
    namingStrategy ??= new CamelCaseNamingStrategy();
    var contractResolver = new DefaultContractResolver { NamingStrategy = namingStrategy };

    return new JsonSerializerSettings { ContractResolver = contractResolver, Formatting = Formatting.Indented };
  }
}
