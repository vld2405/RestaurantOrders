using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestaurantOrders.Database.Migrations
{
    /// <inheritdoc />
    public partial class populatingRestaurantStockEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestaurantStocks_ProductId",
                table: "RestaurantStocks");

            migrationBuilder.InsertData(
                table: "RestaurantStocks",
                columns: new[] { "Id", "ProductId", "StockQuantity" },
                values: new object[,]
                {
                    { 1, 1, 10 },
                    { 2, 2, 10 },
                    { 3, 3, 10 },
                    { 4, 4, 10 },
                    { 5, 5, 10 },
                    { 6, 6, 10 },
                    { 7, 7, 10 },
                    { 8, 8, 10 },
                    { 9, 9, 10 },
                    { 10, 10, 10 },
                    { 11, 11, 10 },
                    { 12, 12, 10 },
                    { 13, 13, 10 },
                    { 14, 14, 10 },
                    { 15, 15, 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantStocks_ProductId",
                table: "RestaurantStocks",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestaurantStocks_ProductId",
                table: "RestaurantStocks");

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "RestaurantStocks",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantStocks_ProductId",
                table: "RestaurantStocks",
                column: "ProductId");
        }
    }
}
