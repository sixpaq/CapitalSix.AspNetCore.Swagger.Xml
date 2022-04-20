# CapitalSix.AspNetCore.Swagger.Xml

This module corrects the XML handling in Swagger in AspNetCore. The default Swagger implementation does not handle XML annotation attributes correctly. This module handles XmlRoot, XmlElement and XmlIgnore attributes in data models.

### How to configure the module.
```c#
void ConfigureServices(IServicesCollection services)
{
    ...
    services.AddSwagger();

    // or use the following if you need additional customization

    services.AddSwaggerGen(c =>
        {
            // Add the response filter. This filter automatically adds
            // 400 and 500 response definition.
            c.OperationFilter<DefaultOperationResponseFilter>();

            // Add the Xml schema filter to handle all Xml annotation
            // attributes
            c.SchemaFilter<XmlSchemaFilter>();
            
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
        });
    ...
}
```

## Sample model
```c#
[XmlRoot("SampleModel")] // Remove the Dto suffix
public class SampleModelDto
{
    [XmlIgnore]
    public int? Id { get; set; }

    [XmlElement("Id")]
    [SwaggerXmlSchemaType(typeof(int))]
    public string? IdAsString
    {
        get => Id.ToString();
        set => Id = int.TryParse(value, out var intValue) ? intValue : default(int);
    }

    public string? Name { get; set; }

    [XmlElement("Extra")]
    public string? Description { get; set; }
}
```
