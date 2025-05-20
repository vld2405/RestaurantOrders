using RestaurantOrders.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantOrders.Database.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Column(TypeName = "decimal(6, 2)")]
        public decimal Price { get; set; }
        public List<Allergen>? Allergens { get; set; }
    }
}
