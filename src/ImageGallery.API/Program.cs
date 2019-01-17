using ImageGallery.API.Entities;
using ImageGallery.API.Helpers;
using ImageGallery.API.Settings;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ImageGallery.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            // migrate & seed the database.  Best practice = in Main, using service scope
            using (var scope = host.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<GalleryContext>();
                    LogConnectionString(scope, logger);

                    await context.Database.MigrateAsync();
                    await context.EnsureSeedDataForContextAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                    throw;
                }
            }

            // run the web app
            host.Run();
        }

        private static void LogConnectionString(IServiceScope scope, ILogger<Program> logger)
        {
            DbSettings dbSettings = scope.ServiceProvider.GetRequiredService<IOptions<DbSettings>>().Value;
            var sanitizedCnnStrng = new SqlConnectionStringBuilder(dbSettings.ConnectionString)
                .SanitizedConnectionString();

            // Console.Out.WriteLine("Connection to db: {0}", dbSettings.ConnectionString);
            
            // not sure whether logging is wired up yet
            logger.LogInformation($"Connection to db: {sanitizedCnnStrng}");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(Startup.ConfigureStartupServices)
                .UseStartup<Startup>();
    }
}
