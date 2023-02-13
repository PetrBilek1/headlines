using Headlines.ORM.Core.Context;
using Headlines.RSSProcessingMicroService.DependencyResolution;
using PBilek.ORM.EntityFrameworkCore.SQL.DependencyResolution;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHealthChecks();

string? connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
    .AddORMDependencyGroup<HeadlinesDbContext>(GetConnectionString(connectionStringTemplate!))
    .AddMicroServiceDependencyGroup()
    .AddMappingDependencyGroup();

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