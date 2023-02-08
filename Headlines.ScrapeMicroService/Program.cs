using Headlines.DependencyResolution;
using Headlines.ScrapeMicroService.DependencyResolution;

namespace Headlines.ScrapeMicroService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHealthChecks();

            string? connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services
                .AddBasicDependencyGroup()
                .AddORMDependencyGroup(GetConnectionString(connectionStringTemplate!))
                .AddMicroServiceDependencyGroup()
                .AddMappingDependencyGroup();

            var app = builder.Build();

            app.MapHealthChecks("/health");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
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
    }
}