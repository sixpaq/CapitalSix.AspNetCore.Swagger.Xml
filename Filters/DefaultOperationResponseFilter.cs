using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CapitalSix.AspNetCore.Swagger.Xml;

/// <summary>
/// This filter will automatically add response descriptions
/// for HTTP status codes 400 and 500
/// </summary>
public class DefaultOperationResponseFilter : IOperationFilter
{
    /// <summary>
    /// Add response descriptions for HTTP status codes 400 and 500
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="context"></param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var responseType = typeof(ProblemDetails);

        var schema = context.SchemaGenerator.GenerateSchema(responseType, context.SchemaRepository);
        
        var errorResponse400 = new OpenApiResponse()
        {
            Description = "Bad Request",
            Content = new Dictionary<string, OpenApiMediaType>()
        };
        errorResponse400.Content.Add("application/problem+json", new OpenApiMediaType()
        {
            Schema = schema
        });
        errorResponse400.Content.Add("application/json", new OpenApiMediaType()
        {
            Schema = schema
        });
        operation.Responses.Add("400", errorResponse400);

        var errorResponse500 = new OpenApiResponse()
        {
            Description = "Internal Server Error",
            Content = new Dictionary<string, OpenApiMediaType>()
        };
        errorResponse500.Content.Add("application/problem+json", new OpenApiMediaType()
        {
            Schema = schema
        });
        errorResponse500.Content.Add("application/json", new OpenApiMediaType()
        {
            Schema = schema
        });
        operation.Responses.Add("500", errorResponse500);
    }
}