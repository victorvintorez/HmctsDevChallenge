using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace HmctsDevChallenge.Backend.Services.OpenApiTransformer;

public class OpenApiSchemaTransformer(ILogger<OpenApiSchemaTransformer> logger) : IOpenApiSchemaTransformer
{
    public const string DtoNamespace = "HmctsDevChallenge.Backend.Models.Dto";

    Task IOpenApiSchemaTransformer.TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo is not { Kind: JsonTypeInfoKind.Object, Type: { } type } || !IsDtoType(type))
            return Task.CompletedTask;

        try
        {
            var schemaName = BuildSchemaName(type);
            var schemaId = ToCamelCase(schemaName);

            schema.Title = schemaName;

            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("Transformed schema: {TypeFullName} -> Title: {SchemaName}, Id: {SchemaId}",
                    type.FullName, schemaName, schemaId);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to transform schema for type {TypeFullName}", type.FullName);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Checks if a type is a DTO type (under LicenceManager.Server.Models.Dto namespace).
    /// </summary>
    private static bool IsDtoType(Type type)
    {
        // For generic types, check if any type argument is a DTO type
        if (type.IsGenericType) return type.GetGenericArguments().Any(IsDtoType);

        // Check if the type's namespace starts with the DTO namespace
        var typeNamespace = type.Namespace ?? string.Empty;
        return typeNamespace.StartsWith(DtoNamespace, StringComparison.Ordinal);
    }

    /// <summary>
    ///     Builds a schema name from a type by extracting the portion after "Dto" in the namespace
    ///     and concatenating all type names (including nested types).
    /// </summary>
    public static string BuildSchemaName(Type type)
    {
        return type.IsGenericType ? BuildGenericSchemaName(type) : BuildNonGenericSchemaName(type);
    }

    /// <summary>
    ///     Builds a schema name for generic types like PaginatedList&lt;ApiKeyDto.Read&gt;.
    /// </summary>
    private static string BuildGenericSchemaName(Type type)
    {
        var genericTypeDef = type.GetGenericTypeDefinition();
        var genericTypeName = CleanTypeName(genericTypeDef.Name);

        // Get the schema names for all generic arguments
        var genericArgs = type.GetGenericArguments()
            .Select(BuildSchemaName)
            .ToArray();

        // Join with "Of" for single argument, "And" for multiple
        var argsString = string.Join("And", genericArgs);

        return $"{genericTypeName}Of{argsString}";
    }

    /// <summary>
    ///     Builds a schema name for non-generic types.
    ///     Extracts everything after "Dto" in the namespace and concatenates with type names.
    /// </summary>
    private static string BuildNonGenericSchemaName(Type type)
    {
        var parts = new List<string>();

        // Extract namespace segments after "Dto"
        var typeNamespace = type.Namespace ?? string.Empty;
        if (typeNamespace.StartsWith(DtoNamespace, StringComparison.Ordinal))
        {
            var afterDto = typeNamespace[DtoNamespace.Length..];
            if (afterDto.StartsWith('.')) afterDto = afterDto[1..];

            if (!string.IsNullOrEmpty(afterDto))
            {
                var namespaceParts = afterDto.Split('.');
                parts.AddRange(namespaceParts);
            }
        }

        // Add declaring types (for nested types) from outermost to innermost
        var declaringTypes = new List<Type>();
        var declaringType = type.DeclaringType;
        while (declaringType != null)
        {
            declaringTypes.Insert(0, declaringType);
            declaringType = declaringType.DeclaringType;
        }

        parts.AddRange(declaringTypes.Select(dt => CleanTypeName(dt.Name)));

        // Add the type itself
        parts.Add(CleanTypeName(type.Name));

        return string.Concat(parts);
    }

    /// <summary>
    ///     Removes generic arity marker (e.g., `1) from type names.
    /// </summary>
    private static string CleanTypeName(string typeName)
    {
        var backtickIndex = typeName.IndexOf('`');
        return backtickIndex >= 0 ? typeName[..backtickIndex] : typeName;
    }

    /// <summary>
    ///     Converts a PascalCase string to camelCase.
    /// </summary>
    private static string ToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str[0]))
            return str;

        return char.ToLowerInvariant(str[0]) + str[1..];
    }
}