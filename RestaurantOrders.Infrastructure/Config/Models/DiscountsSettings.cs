using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrders.Infrastructure.Config.Models
{
    public class DiscountsSettings
    {
        public int MenuItemDiscount { get; set; }
        public int LargeOrderDiscount { get; set; }
    }
}
