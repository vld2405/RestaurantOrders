using Microsoft.EntityFrameworkCore;
using RestaurantOrders.Database.Entities;
using RestaurantOrders.Infrastructure.Config;
using System.Reflection.Emit;

namespace RestaurantOrders.Database.Context
{
    public class RestaurantDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Allergen> Allergens { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuDetail> MenuDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<RestaurantStock> RestaurantStocks { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Menu>()
                .HasOne(m => m.Category)
                .WithMany(c => c.Menus)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RestaurantStock>()
                .HasOne(rs => rs.Product)
                .WithOne()
                .HasForeignKey<RestaurantStock>(rs => rs.ProductId)
                .OnDelete(DeleteBehavior.Cascade);


            Seed(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Breakfast"},
                new Category { Id = 2, Name = "Appetizer"},
                new Category { Id = 3, Name = "Soups" },
                new Category { Id = 4, Name = "Dessert" },
                new Category { Id = 5, Name = "Drink" },
                new Category { Id = 6, Name = "Menu" }
            );

            modelBuilder.Entity<Product>().HasData(
                // Breakfast Items
                new Product
                {
                    Id = 1,
                    Name = "Eggs Benedict",
                    Quantity = 100,
                    CategoryId = 1,
                    Price = 35.99M
                },
                new Product
                {
                    Id = 2,
                    Name = "Avocado Toast",
                    Quantity = 100,
                    CategoryId = 1,
                    Price = 28.50M
                },
                new Product
                {
                    Id = 3,
                    Name = "Pancakes with Maple Syrup",
                    Quantity = 100,
                    CategoryId = 1,
                    Price = 25.99M
                },

                // Appetizer Items
                new Product
                {
                    Id = 4,
                    Name = "Bruschetta",
                    Quantity = 100,
                    CategoryId = 2,
                    Price = 22.50M
                },
                new Product
                {
                    Id = 5,
                    Name = "Spinach Artichoke Dip",
                    Quantity = 100,
                    CategoryId = 2,
                    Price = 32.99M
                },
                new Product
                {
                    Id = 6,
                    Name = "Garlic Bread",
                    Quantity = 100,
                    CategoryId = 2,
                    Price = 15.99M
                },

                // Soups
                new Product
                {
                    Id = 7,
                    Name = "Tomato Basil Soup",
                    Quantity = 100,
                    CategoryId = 3,
                    Price = 18.50M
                },
                new Product
                {
                    Id = 8,
                    Name = "Chicken Noodle Soup",
                    Quantity = 100,
                    CategoryId = 3,
                    Price = 22.99M
                },
                new Product
                {
                    Id = 9,
                    Name = "French Onion Soup",
                    Quantity = 100,
                    CategoryId = 3,
                    Price = 25.50M
                },

                // Desserts
                new Product
                {
                    Id = 10,
                    Name = "Tiramisu",
                    Quantity = 100,
                    CategoryId = 4,
                    Price = 28.99M
                },
                new Product
                {
                    Id = 11,
                    Name = "Chocolate Cake",
                    Quantity = 100,
                    CategoryId = 4,
                    Price = 22.50M
                },
                new Product
                {
                    Id = 12,
                    Name = "Crème Brûlée",
                    Quantity = 100,
                    CategoryId = 4,
                    Price = 35.99M
                },

                // Drinks
                new Product
                {
                    Id = 13,
                    Name = "Sparkling Water",
                    Quantity = 100,
                    CategoryId = 5,
                    Price = 8.99M
                },
                new Product
                {
                    Id = 14,
                    Name = "Iced Tea",
                    Quantity = 100,
                    CategoryId = 5,
                    Price = 12.50M
                },
                new Product
                {
                    Id = 15,
                    Name = "House Wine (Glass)",
                    Quantity = 100,
                    CategoryId = 5,
                    Price = 25.99M
                }

            );

            modelBuilder.Entity<RestaurantStock>().HasData(
                new RestaurantStock { Id = 1, ProductId = 1, StockQuantity = 10 },
                new RestaurantStock { Id = 2, ProductId = 2, StockQuantity = 10 },
                new RestaurantStock { Id = 3, ProductId = 3, StockQuantity = 10 },
                new RestaurantStock { Id = 4, ProductId = 4, StockQuantity = 10 },
                new RestaurantStock { Id = 5, ProductId = 5, StockQuantity = 10 },
                new RestaurantStock { Id = 6, ProductId = 6, StockQuantity = 10 },
                new RestaurantStock { Id = 7, ProductId = 7, StockQuantity = 10 },
                new RestaurantStock { Id = 8, ProductId = 8, StockQuantity = 10 },
                new RestaurantStock { Id = 9, ProductId = 9, StockQuantity = 10 },
                new RestaurantStock { Id = 10, ProductId = 10, StockQuantity = 10 },
                new RestaurantStock { Id = 11, ProductId = 11, StockQuantity = 10 },
                new RestaurantStock { Id = 12, ProductId = 12, StockQuantity = 10 },
                new RestaurantStock { Id = 13, ProductId = 13, StockQuantity = 10 },
                new RestaurantStock { Id = 14, ProductId = 14, StockQuantity = 10 },
                new RestaurantStock { Id = 15, ProductId = 15, StockQuantity = 10 }
            );
        }
    }
}