using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace WalletAPI.Swagger;

/// <summary>
/// Swagger schema filter to modify description of enum types so they
/// show the XML docs attached to each member of the enum.
/// </summary>
public class SwaggerDescribeEnumMembers : ISchemaFilter
{
    private readonly XDocument _xmlComments;
    private readonly string _assemblyName;

    /// <summary>
    /// Initialize schema filter.
    /// </summary>
    /// <param name="xmlComments">Document containing XML docs for enum members.</param>
    public SwaggerDescribeEnumMembers(XDocument xmlComments)
    {
        _xmlComments = xmlComments;
        _assemblyName = DetermineAssembly(xmlComments);
    }

    /// <summary>
    /// Pre-amble to use before the enum items
    /// </summary>
    public static string Prefix { get; set; } = "<p>Possible values:</p>";

    /// <summary>
    /// Format to use, 0 : value, 1: Name, 2: Description
    /// </summary>
    public static string Format { get; set; } = "<b>{0} - {1}</b>: {2}";

    /// <summary>
    /// Apply this schema filter.
    /// </summary>
    /// <param name="schema">Target schema object.</param>
    /// <param name="context">Schema filter context.</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        // Only process enums and...
        if (!type.IsEnum)
        {
            return;
        }

        // Enable next section for only the comments defined in their origin assembly
        /*if (type.Assembly.GetName().Name != _assemblyName)
      {
        return;
      }*/
        var sb = new StringBuilder(schema.Description);

        if (!string.IsNullOrEmpty(Prefix))
        {
            sb.AppendLine(Prefix);
        }

        sb.AppendLine("<ul>");

        // TODO: Handle flags better e.g. Hex formatting
        foreach (var name in Enum.GetValues(type))
        {
            // Allows for large enums
            var value = Convert.ToInt64(name);
            var fullName = $"F:{type.FullName}.{name}";

            var description = _xmlComments.XPathEvaluate(
              $"normalize-space(//member[@name = '{fullName}']/summary/text())"
            ) as string;

            sb.AppendLine(string.Format("<li>" + Format + "</li>", value, name, description));
        }

        sb.AppendLine("</ul>");

        schema.Description = sb.ToString();
    }

    private string DetermineAssembly(XDocument doc)
    {
        var name = ((IEnumerable)doc.XPathEvaluate("/doc/assembly")).Cast<XElement>().ToList().FirstOrDefault();

        return name?.Value;
    }
}
