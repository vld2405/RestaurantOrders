using Microsoft.EntityFrameworkCore;
using RestaurantOrders.Database.Entities;
using RestaurantOrders.Infrastructure.Config;

namespace RestaurantOrders.Database.Context
{
    public class RestaurantDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Allergen> Allergens { get; set; }

        public RestaurantDbContext() { }

        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase);
            }
        }
    }
}