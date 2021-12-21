using Vesuvio.DatabaseMigration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using WeddingNepal.WebAPI.Helpers;

namespace Vesuvio.API
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var hostBuilder = CreateHostBuilder(args);
            var host = hostBuilder.Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDatabaseContext>();
                    DbInitializer.InitializeAsync(context, services).Wait();
                }
                catch (Exception ex)
                {
                    //NLog: catch setup errors
                    throw;
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
