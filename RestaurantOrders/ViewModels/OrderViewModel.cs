using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RestaurantOrders.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string OrderState { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EstimatedDeliveryTime { get; set; }
        public decimal TotalOrderValue { get; set; }
        public bool CanCancel { get; set; }
        public Brush StatusColor { get; set; }
    }
}
