namespace RestaurantOrders.Database.Entities
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public int? OrderId { get; set; }
        public int? MenuId { get; set; }

        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public Menu Menu { get; set; } = null!;
    }
}