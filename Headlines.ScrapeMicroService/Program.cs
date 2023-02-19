using Headlines.BL.Implementations.MessageBroker;
using Headlines.ORM.Core.Context;
using Headlines.ScrapeMicroService.DependencyResolution;
using PBilek.ObjectStorageService;
using PBilek.ORM.EntityFrameworkCore.SQL.DependencyResolution;

namespace Headlines.ScrapeMicroService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHealthChecks();
            builder.Services.AddHttpClient();

            string? connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddORMDependencyGroup<HeadlinesDbContext>(GetConnectionString(connectionStringTemplate!));

            builder.Services.AddMicroServiceDependencyGroup();
            builder.Services.AddObjectStorageDependencyGroup(GetObjectStorageConfiguration());
            builder.Services.AddMappingDependencyGroup();
            builder.Services.AddMessageQueueDependencyGroup(GetMessageBrokerSettings());

            var app = builder.Build();

            app.MapHealthChecks("/health");

            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.Run();
        }

        private static string GetConnectionString(string template)
        {
            template = template.Replace("{DB_LOGIN}", Environment.GetEnvironmentVariable("DB_LOGIN"));
            template = template.Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD"));
            template = template.Replace("{DB_DATA_SOURCE}", Environment.GetEnvironmentVariable("DB_DATA_SOURCE"));
            template = template.Replace("{DB_INITIAL_CATALOG}", Environment.GetEnvironmentVariable("DB_INITIAL_CATALOG"));

            return template;
        }

        private static MessageBrokerSettings GetMessageBrokerSettings()
        {
            return new MessageBrokerSettings
            {
                Host = Environment.GetEnvironmentVariable("MQ_HOST") ?? string.Empty,
                Username = Environment.GetEnvironmentVariable("MQ_USERNAME") ?? string.Empty,
                Password = Environment.GetEnvironmentVariable("MQ_PASSWORD") ?? string.Empty
            };
        }

        private static ObjectStorageConfiguration GetObjectStorageConfiguration()
        {
            return new ObjectStorageConfiguration
            {
                ServiceUrl = Environment.GetEnvironmentVariable("OS_URL") ?? string.Empty,
                AccessKey = Environment.GetEnvironmentVariable("OS_ACCESS_KEY") ?? string.Empty,
                SecretKey = Environment.GetEnvironmentVariable("OS_SECRET_KEY") ?? string.Empty,
            };
        }
    }
}