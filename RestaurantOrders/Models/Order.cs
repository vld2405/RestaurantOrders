using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrders.Models
{
    public class Order
    {
        public User? User { get; set; }
        public List<Tuple<Product, int>>? Products{ get; set; }

        public Order() {}

        public Order(User User, List<Tuple<Product, int>>? Products)
        {
            this.User = User;
            this.Products = Products;
        }
    }
}
