using RestaurantOrders.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrders.Database.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<OrderDetail> OrderDetails { get; set; } = null!;
        public OrderState OrderState { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EstimatedDeliveryTime { get; set; }
    }
}
