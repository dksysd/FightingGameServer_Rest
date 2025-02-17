using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FightingGameServer_Rest.Swagger;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // ReSharper disable once InvertIf
        if (context.Type.IsEnum) // 타입이 Enum인지 확인
        {
            schema.Type = "string"; // 스키마 타입을 string으로 변경
            schema.Enum = new List<IOpenApiAny>(); // Enum 값 목록 초기화

            // Enum 멤버 이름들을 OpenApiString으로 변환하여 schema.Enum에 추가
            foreach (string enumName in Enum.GetNames(context.Type))
            {
                schema.Enum.Add(new OpenApiString(enumName));
            }
        }
    }
}