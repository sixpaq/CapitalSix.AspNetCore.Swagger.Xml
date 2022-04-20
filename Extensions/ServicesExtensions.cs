using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace CapitalSix.AspNetCore.Swagger.Xml;

/// <summary>
/// This extension class provides a couple of extension
/// methods that will be used in the startup configuration
/// of each logical app.
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// This extension method configures the swagger engine which will
    /// provide an OpenAPI for the logical app
    /// </summary>
    /// <param name="services">The services collection</param>
    /// <returns>The services collection</returns>
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.OperationFilter<DefaultOperationResponseFilter>();
            
            c.CustomSchemaIds(currentClass =>
            {
                // This custom schema respects the use of the XmlRootAttribute
                // for the schema's in Swagger. If a model uses the
                // XmlRootAttribute, it's name will be represented in the OpenAPI.
                var rootAttribute = currentClass.GetCustomAttributes()
                    .OfType<XmlRootAttribute>()
                    .SingleOrDefault();
                var className = rootAttribute?.ElementName ?? currentClass.Name;
                return className;
            });

            // The name of the startup assembly
            var assemblyName = Assembly.GetEntryAssembly()?.GetName()?.Name;
            
            // The path to the xml documentation which is generated during the build
            var filePath = Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.xml");
            c.IncludeXmlComments(filePath);
            
            c.SchemaFilter<XmlSchemaFilter>();
        });
        return services;
    }
}
