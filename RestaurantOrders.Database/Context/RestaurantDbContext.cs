using Microsoft.EntityFrameworkCore;
using RestaurantOrders.Database.Entities;
using RestaurantOrders.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrders.Database.Context
{
    public class RestaurantDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetail> OrderDetails{ get; set; }
        public DbSet<Allergen> Allergens{ get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"data source=localhost;Initial Catalog=RestaurantOrders;Persist Security Info=True;User ID=hatz;Password=1234;Connection Timeout=60;TrustServerCertificate=True");
            optionsBuilder.UseSqlServer(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase);
        }
    }
}
