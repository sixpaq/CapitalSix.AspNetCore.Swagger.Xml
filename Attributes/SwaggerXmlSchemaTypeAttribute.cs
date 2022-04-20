namespace CapitalSix.AspNetCore.Swagger.Xml;

public class SwaggerXmlSchemaTypeAttribute : Attribute
{
    public SwaggerXmlSchemaTypeAttribute(Type type)
    {
        Type = type;
    }
    
    public Type Type { get; private set; }
}