﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RestaurantOrders.Database.Context;

#nullable disable

namespace RestaurantOrders.Database.Migrations
{
    [DbContext(typeof(RestaurantDbContext))]
    [Migration("20250520112904_addedImagesAndRestaurantStocksEnitities2")]
    partial class addedImagesAndRestaurantStocksEnitities2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AllergenProduct", b =>
                {
                    b.Property<int>("AllergensId")
                        .HasColumnType("int");

                    b.Property<int>("ProductsId")
                        .HasColumnType("int");

                    b.HasKey("AllergensId", "ProductsId");

                    b.HasIndex("ProductsId");

                    b.ToTable("AllergenProduct");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.Allergen", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Allergens");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Breakfast"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Appetizer"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Soups"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Dessert"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Drink"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Menu"
                        });
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.Images", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.Menu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(6, 2)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Menus");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.MenuDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("MenuId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MenuId");

                    b.HasIndex("ProductId");

                    b.ToTable("MenuDetails");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EstimatedDeliveryTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("OrderState")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.OrderDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("MenuId")
                        .HasColumnType("int");

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MenuId");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(6, 2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CategoryId = 1,
                            Name = "Eggs Benedict",
                            Price = 35.99m,
                            Quantity = 100
                        },
                        new
                        {
                            Id = 2,
                            CategoryId = 1,
                            Name = "Avocado Toast",
                            Price = 28.50m,
                            Quantity = 100
                        },
                        new
                        {
                            Id = 3,
                            CategoryId = 1,
                            Name = "Pancakes with Maple Syrup",
                            Price = 25.99m,
                            Quantity = 100
                        },
                        new
                        {
                            Id = 4,
                            CategoryId = 2,
                            Name = "Bruschetta",
                            Price = 22.50m,
                            Quantity = 100
                        },
                        new
                        {
                            Id = 5,
                            CategoryId = 2,
                            Name = "Spinach Artichoke Dip",
                            Price = 32.99m,
                            Quantity = 100
                        },
                        new
                        {
                            Id = 6,
                            CategoryId = 2,
                            Name = "Garlic Bread",
                            Price = 15.99m,
                            Quantity = 100
                        },
                        new
                        {
                            Id = 7,
                            CategoryId = 3,
                            Name = "Tomato Basil Soup",
                            Price = 18.50m,
                            Quantity = 100
                        },
                        new
                        {
                            Id = 8,
                            CategoryId = 3,
                            Name = "Chicken Noodle Soup",
                            Price = 22.99m,
                            Quantity = 100
                        },
                        new
                        {
                            Id = 9,
                            CategoryId = 3,
                            Name = "French Onion Soup",
                            Price = 25.50m,
                            Quantity = 100
                        },
                        new
                        {
                            Id = 10,
                            CategoryId = 4,
                            Name = "Tiramisu",
                            Price = 28.99m,
                            Quantity = 100
                        },
                        new
                        {
                            Id = 11,
                            CategoryId = 4,
                            Name = "Chocolate Cake",
                            Price = 22.50m,
                            Quantity = 100
                        },
                        new
                        {
                            Id = 12,
                            CategoryId = 4,
                            Name = "Crème Brûlée",
                            Price = 35.99m,
                            Quantity = 100
                        },
                        new
                        {
                            Id = 13,
                            CategoryId = 5,
                            Name = "Sparkling Water",
                            Price = 8.99m,
                            Quantity = 100
                        },
                        new
                        {
                            Id = 14,
                            CategoryId = 5,
                            Name = "Iced Tea",
                            Price = 12.50m,
                            Quantity = 100
                        },
                        new
                        {
                            Id = 15,
                            CategoryId = 5,
                            Name = "House Wine (Glass)",
                            Price = 25.99m,
                            Quantity = 100
                        });
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.RestaurantStock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("StockQuantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("RestaurantStocks");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AllergenProduct", b =>
                {
                    b.HasOne("RestaurantOrders.Database.Entities.Allergen", null)
                        .WithMany()
                        .HasForeignKey("AllergensId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RestaurantOrders.Database.Entities.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.Images", b =>
                {
                    b.HasOne("RestaurantOrders.Database.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.Menu", b =>
                {
                    b.HasOne("RestaurantOrders.Database.Entities.Category", "Category")
                        .WithMany("Menus")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.MenuDetail", b =>
                {
                    b.HasOne("RestaurantOrders.Database.Entities.Menu", "Menu")
                        .WithMany("MenuDetails")
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RestaurantOrders.Database.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Menu");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.Order", b =>
                {
                    b.HasOne("RestaurantOrders.Database.Entities.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.OrderDetail", b =>
                {
                    b.HasOne("RestaurantOrders.Database.Entities.Menu", "Menu")
                        .WithMany()
                        .HasForeignKey("MenuId");

                    b.HasOne("RestaurantOrders.Database.Entities.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId");

                    b.HasOne("RestaurantOrders.Database.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.Navigation("Menu");

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.Product", b =>
                {
                    b.HasOne("RestaurantOrders.Database.Entities.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.RestaurantStock", b =>
                {
                    b.HasOne("RestaurantOrders.Database.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.Category", b =>
                {
                    b.Navigation("Menus");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.Menu", b =>
                {
                    b.Navigation("MenuDetails");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("RestaurantOrders.Database.Entities.User", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
