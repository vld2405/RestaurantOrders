using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrders.Database.Entities
{
    public class MenuDetail
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public int MenuId { get; set; }

        public Menu Menu { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
