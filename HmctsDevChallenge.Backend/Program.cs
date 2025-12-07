using System.Configuration;
using System.Text.Json.Serialization;
using HmctsDevChallenge.Backend.Database;
using HmctsDevChallenge.Backend.Services.OpenApiTransformer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.File(
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "HmctsDevChallenge", "Backend", "Logs", "log-.txt"),
        rollingInterval: RollingInterval.Day)
    .WriteTo.Console(new ExpressionTemplate(
        "[{@t:HH:mm:ss} {@l:u3} ({SourceContext})] {@m}\n{@x}",
        theme: TemplateTheme.Code
    )));

// Database
if (!builder.Environment.IsEnvironment("Test"))
    builder.Services.AddDbContextPool<HmctsDbContext>(opts =>
    {
        opts.UseNpgsql(builder.Configuration.GetConnectionString("Database")
                       ?? throw new ConfigurationErrorsException("ConnectionStrings.Database is required!"));
    });

// CORS
builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
        policy.WithOrigins(
                builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                ?? throw new ConfigurationErrorsException("A list of AllowedOrigins is required!"))
            .AllowAnyMethod()
            .AllowAnyHeader());
    opts.AddPolicy("OpenAPIPolicy", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// JSON Options
builder.Services.Configure<JsonOptions>(opts =>
{
    // Handle circular references (fallback)
    opts.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    opts.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Routing
builder.Services.AddRouting(opts =>
{
    opts.LowercaseUrls = true;
    opts.LowercaseQueryStrings = true;
});

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        // Handle circular references (fallback)
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// OpenAPI Document Generation
builder.Services.AddOpenApi(opts =>
{
    // Customize schema generation to use qualified names for clarity
    opts.AddSchemaTransformer<OpenApiSchemaTransformer>();
    opts.CreateSchemaReferenceId = jsonTypeInfo =>
    {
        var type = jsonTypeInfo.Type;
        // Check if this is a DTO type
        var typeNamespace = type.Namespace ?? string.Empty;
        return !typeNamespace.StartsWith(OpenApiSchemaTransformer.DtoNamespace, StringComparison.Ordinal)
            ?
            // Not a DTO, use default behavior
            OpenApiOptions.CreateDefaultSchemaReferenceId(jsonTypeInfo)
            :
            // For DTO types, generate the fully qualified name
            OpenApiSchemaTransformer.BuildSchemaName(type);
    };
});

var app = builder.Build();

// Run database migrations (skip in test environment)
if (!app.Environment.IsEnvironment("Test"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetService<HmctsDbContext>();
    if (db is not null)
        await db.Database.MigrateAsync();
    else
        throw new Exception("Could not migrate the database!");
}

// Use statements
app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

// Map OpenAPI + OpenAPI Viewer (Scalar)
app.MapOpenApi("/api/openapi/{documentName}.json")
    .CacheOutput()
    .RequireCors("OpenAPIPolicy");
app.MapScalarApiReference("/api/docs/", opts =>
{
    opts.WithTitle("HMCTS DTS Dev Challenge: API Documentation");
    opts.WithOpenApiRoutePattern("/api/openapi/{documentName}.json");
    opts.WithTheme(ScalarTheme.BluePlanet);
}).RequireCors("OpenAPIPolicy");

app.MapControllers();

// Serving the SPA
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapWhen(
    ctx => !ctx.Request.Path.StartsWithSegments("/api"),
    fallback =>
        fallback.UseEndpoints(endpoint => endpoint.MapFallbackToFile("index.html")
        ));

app.Run();

public partial class Program;