using Headlines.BL.Implementations.MessageBroker;
using Headlines.ORM.Core.Context;
using Headlines.WebAPI.Configs;
using Headlines.WebAPI.DependencyResolution;
using Headlines.WebAPI.Middlewares.WebSocketServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using PBilek.ObjectStorageService;
using PBilek.ORM.EntityFrameworkCore.SQL.DependencyResolution;

var builder = WebApplication.CreateBuilder(args);
var corsPolicyName = "corsPolicyName";

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName,
        policy =>
        {
            policy.WithOrigins(GetCorsOrigins().ToArray()).AllowAnyHeader().AllowAnyMethod();
        });
});

builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = ApiVersion.Default;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

string connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

builder.Services.AddORMDependencyGroup<HeadlinesDbContext>(GetConnectionString(connectionStringTemplate));
builder.Services.AddWebAPIDependencyGroup();
builder.Services.AddObjectStorageDependencyGroup(GetObjectStorageConfiguration());
builder.Services.AddMessageQueueDependencyGroup(GetMessageBrokerSettings());
builder.Services.AddMappingDependencyGroup();
builder.Services.AddRateLimiterDependencyGroup();
builder.Services.AddWebSocketServerDependencyGroup();

string redisConnectionStringTemplate = builder.Configuration.GetConnectionString("Redis") ?? string.Empty;
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = GetRedisConnectionString(redisConnectionStringTemplate);
    options.InstanceName = "headlines-webapi-";
});

var app = builder.Build();

var dbContext = app.Services.GetRequiredService<HeadlinesDbContext>();
await dbContext.Database.MigrateAsync();

app.UseCors(corsPolicyName);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        IApiVersionDescriptionProvider descriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in descriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseAuthorization();

app.UseRateLimiter();

app.UseWebSockets();
app.UseWebSocketServer();

app.MapControllers();

app.Run();

string GetConnectionString(string template)
{
    template = template.Replace("{DB_LOGIN}", Environment.GetEnvironmentVariable("DB_LOGIN"));
    template = template.Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD"));
    template = template.Replace("{DB_DATA_SOURCE}", Environment.GetEnvironmentVariable("DB_DATA_SOURCE"));
    template = template.Replace("{DB_INITIAL_CATALOG}", Environment.GetEnvironmentVariable("DB_INITIAL_CATALOG"));

    return template;
}

string GetRedisConnectionString(string template)
{
    template = template.Replace("{REDIS_HOST}", Environment.GetEnvironmentVariable("REDIS_HOST"));
    template = template.Replace("{REDIS_PASSWORD}", Environment.GetEnvironmentVariable("REDIS_PASSWORD"));

    return template;
}

MessageBrokerSettings GetMessageBrokerSettings()
{
    return new MessageBrokerSettings
    {
        Host = Environment.GetEnvironmentVariable("MQ_HOST") ?? string.Empty,
        Username = Environment.GetEnvironmentVariable("MQ_USERNAME") ?? string.Empty,
        Password = Environment.GetEnvironmentVariable("MQ_PASSWORD") ?? string.Empty,
        ReplicaName = Environment.GetEnvironmentVariable("REPLICA_NAME") ?? string.Empty
    };
}

ObjectStorageConfiguration GetObjectStorageConfiguration()
{
    return new ObjectStorageConfiguration
    {
        ServiceUrl = Environment.GetEnvironmentVariable("OS_URL") ?? string.Empty,
        AccessKey = Environment.GetEnvironmentVariable("OS_ACCESS_KEY") ?? string.Empty,
        SecretKey = Environment.GetEnvironmentVariable("OS_SECRET_KEY") ?? string.Empty,
    };
}

IEnumerable<string> GetCorsOrigins()
{
    var webUiDomain = Environment.GetEnvironmentVariable("WEB_UI_DOMAIN");

    if (webUiDomain == null)
        yield break;

    if (webUiDomain.Contains("localhost"))
    {
        yield return "http://localhost:8081";
        yield return "http://www.localhost:8081";
    }
    else
    {
        yield return $"https://{webUiDomain}";
        yield return $"https://www.{webUiDomain}";
    }
}