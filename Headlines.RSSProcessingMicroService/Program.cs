using Headlines.BL.Implementations.MessageBroker;
using Headlines.ORM.Core.Context;
using Headlines.RSSProcessingMicroService.DependencyResolution;
using PBilek.ObjectStorageService;
using PBilek.ORM.EntityFrameworkCore.SQL.DependencyResolution;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHealthChecks();

string? connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddORMDependencyGroup<HeadlinesDbContext>(GetConnectionString(connectionStringTemplate!));
builder.Services.AddMessageQueueDependencyGroup(GetMessageBrokerSettings());
builder.Services.AddMicroServiceDependencyGroup(GetObjectStorageConfiguration());
builder.Services.AddMappingDependencyGroup();

var app = builder.Build();

app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

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

ObjectStorageConfiguration GetObjectStorageConfiguration()
{
    return new ObjectStorageConfiguration
    {
        ServiceUrl = Environment.GetEnvironmentVariable("OS_URL") ?? string.Empty,
        AccessKey = Environment.GetEnvironmentVariable("OS_ACCESS_KEY") ?? string.Empty,
        SecretKey = Environment.GetEnvironmentVariable("OS_SECRET_KEY") ?? string.Empty,
    };
}