using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestaurantOrders.Database.Migrations
{
    /// <inheritdoc />
    public partial class populatedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Name", "Price", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, "Eggs Benedict", 35.99m, 100 },
                    { 2, 1, "Avocado Toast", 28.50m, 100 },
                    { 3, 1, "Pancakes with Maple Syrup", 25.99m, 100 },
                    { 4, 2, "Bruschetta", 22.50m, 100 },
                    { 5, 2, "Spinach Artichoke Dip", 32.99m, 100 },
                    { 6, 2, "Garlic Bread", 15.99m, 100 },
                    { 7, 3, "Tomato Basil Soup", 18.50m, 100 },
                    { 8, 3, "Chicken Noodle Soup", 22.99m, 100 },
                    { 9, 3, "French Onion Soup", 25.50m, 100 },
                    { 10, 4, "Tiramisu", 28.99m, 100 },
                    { 11, 4, "Chocolate Cake", 22.50m, 100 },
                    { 12, 4, "Crème Brûlée", 35.99m, 100 },
                    { 13, 5, "Sparkling Water", 8.99m, 100 },
                    { 14, 5, "Iced Tea", 12.50m, 100 },
                    { 15, 5, "House Wine (Glass)", 25.99m, 100 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);
        }
    }
}
