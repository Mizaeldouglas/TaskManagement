using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TaskManagement.Api.Swagger
{
    public class CustomSchemaFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var errorResponseSchema = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["message"] = new OpenApiSchema { Type = "string" }
                },
                Example = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["message"] = new Microsoft.OpenApi.Any.OpenApiString("Mensagem de erro")
                }
            };

            swaggerDoc.Components.Schemas["ErrorResponse"] = errorResponseSchema;

            foreach (var pathItem in swaggerDoc.Paths.Values)
            {
                foreach (var operation in pathItem.Operations.Values)
                {
                    if (operation.Responses.ContainsKey("400"))
                    {
                        operation.Responses["400"] = new OpenApiResponse
                        {
                            Description = "Bad Request",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "ErrorResponse"
                                        }
                                    },
                                    Example = new Microsoft.OpenApi.Any.OpenApiObject
                                    {
                                        ["message"] = new Microsoft.OpenApi.Any.OpenApiString("Requisição inválida")
                                    }
                                }
                            }
                        };
                    }

                    if (operation.Responses.ContainsKey("404"))
                    {
                        operation.Responses["404"] = new OpenApiResponse
                        {
                            Description = "Not Found",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.Schema,
                                            Id = "ErrorResponse"
                                        }
                                    },
                                    Example = new Microsoft.OpenApi.Any.OpenApiObject
                                    {
                                        ["message"] = new Microsoft.OpenApi.Any.OpenApiString("Recurso não encontrado")
                                    }
                                }
                            }
                        };
                    }
                }
            }
        }
    }
}