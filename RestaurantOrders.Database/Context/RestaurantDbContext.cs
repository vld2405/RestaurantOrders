using Microsoft.EntityFrameworkCore;
using RestaurantOrders.Database.Entities;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //TODO 2: Schimba cu un App.config
            optionsBuilder.UseSqlServer(@"data source=localhost;Initial Catalog=RestaurantOrders;Persist Security Info=True;User ID=hatz;Password=1234;Connection Timeout=60;TrustServerCertificate=True");
        }
    }
}
