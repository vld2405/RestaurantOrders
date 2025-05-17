using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using RestaurantOrders.Database.Context;
using RestaurantOrders.Infrastructure.Config;
using System.IO;

namespace RestaurantOrders.Database
{
    public class RestaurantDbContextFactory : IDesignTimeDbContextFactory<RestaurantDbContext>
    {
        public RestaurantDbContext CreateDbContext(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // Initialize AppConfig
            AppConfig.Init(configuration);

            var optionsBuilder = new DbContextOptionsBuilder<RestaurantDbContext>();
            optionsBuilder.UseSqlServer(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase);

            return new RestaurantDbContext(optionsBuilder.Options);
        }
    }
}