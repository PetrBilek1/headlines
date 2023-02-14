using Headlines.BL.Implementations.MessageBroker;
using Headlines.ORM.Core.Context;
using Headlines.WebAPI.Configs;
using Headlines.WebAPI.DependencyResolution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddMessageQueueDependencyGroup(GetMessageBrokerSettings());
builder.Services.AddMappingDependencyGroup();
builder.Services.AddRateLimiterDependencyGroup();

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

MessageBrokerSettings GetMessageBrokerSettings()
{
    return new MessageBrokerSettings
    {
        Host = Environment.GetEnvironmentVariable("MQ_HOST") ?? string.Empty,
        Username = Environment.GetEnvironmentVariable("MQ_USERNAME") ?? string.Empty,
        Password = Environment.GetEnvironmentVariable("MQ_PASSWORD") ?? string.Empty
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