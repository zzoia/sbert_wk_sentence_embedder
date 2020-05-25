using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TextClustering.Domain;

namespace TextClustering.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var serviceScope = GetScope())
            {
            }
        }

        private static IServiceScope GetScope()
        {
            IConfiguration configuration =
                new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

            var connectionString = configuration.GetConnectionString("default");

            IServiceProvider serviceProvider = new ServiceCollection().AddScoped(_ => configuration)
                .AddLogging(builder => builder
                    .AddConsole(options => options.DisableColors = true)
                    .SetMinimumLevel(LogLevel.Information))
                .AddSingleton(configuration)
                .AddDbContext<TextClusteringDbContext>(options =>
                    options.UseSqlServer(connectionString, x => x.MigrationsAssembly("TextClustering.Migrations")))
                .BuildServiceProvider();

            return serviceProvider.CreateScope();
        }
    }
}