using System.Reflection;
using System.Xml.Serialization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CapitalSix.AspNetCore.Swagger.Xml;

/// <summary>
/// This schema filter makes swagger respect the Xml attributes in
/// Xml schemas. Xml schemas are representations of models in the
/// swagger documentation and in the examples.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public class XmlSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// This method will check each property in a schema for
    /// a XmlElementAttribute and use its elementName instead of
    /// the property name.
    /// </summary>
    /// <param name="schema">The OpenAPI schema</param>
    /// <param name="context">The current schema context</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null) return;
 
        var properties = schema.Properties
            .Select(schemaProperty =>
            {
                var property = context.Type.GetProperties()
                    .Single(p => string.Compare(p.Name, schemaProperty.Key, StringComparison.CurrentCultureIgnoreCase) == 0);
                var customAttributes = property.GetCustomAttributes().ToArray();
                var xmlElementAttribute = customAttributes
                    .OfType<XmlElementAttribute>()
                    .SingleOrDefault();
                var swaggerReferenceTypeAttribute = customAttributes
                    .OfType<SwaggerXmlSchemaTypeAttribute>()
                    .SingleOrDefault();
                var elementName = xmlElementAttribute?.ElementName ?? property.Name;
                UpdateSchema(schemaProperty.Value, elementName, swaggerReferenceTypeAttribute);
                return (Property: property, ElementAttribute: xmlElementAttribute, ElementName: elementName, Schema: schemaProperty.Value);
            })
            .Where(x => !x.Property.GetCustomAttributes()
                .OfType<XmlIgnoreAttribute>()
                .Any());
 
        schema.Properties = properties
            .ToDictionary(property => property.ElementName, property => property.Schema);
    }

    private void UpdateSchema(OpenApiSchema schema, string elementName, SwaggerXmlSchemaTypeAttribute? swaggerReferenceTypeAttribute)
    {
        schema.Title = elementName;
        schema.Xml = new OpenApiXml
        {
            Name = elementName
        };

        if (swaggerReferenceTypeAttribute?.Type != null)
        {
            if (swaggerReferenceTypeAttribute.Type == typeof(Int32) || swaggerReferenceTypeAttribute.Type == typeof(Int64))
            {
                schema.Type = "integer";
            }
            else if (swaggerReferenceTypeAttribute.Type == typeof(bool))
            {
                schema.Type = "boolean";
            }
            else if (swaggerReferenceTypeAttribute.Type == typeof(float) || swaggerReferenceTypeAttribute.Type == typeof(double) || swaggerReferenceTypeAttribute.Type == typeof(decimal))
            {
                schema.Type = "number";
            }
            else if (swaggerReferenceTypeAttribute.Type == typeof(string))
            {
                schema.Type = "string";
            }
            else if (swaggerReferenceTypeAttribute.Type.IsArray)
            {
                schema.Type = "array";
            }
        }
    }
}