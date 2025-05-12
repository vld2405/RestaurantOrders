using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrders.Models
{
    public class Product
    {
        public string? Name { get; set; }

        public Product() {}

        public Product(string Name)
        {
            this.Name = Name;
        }
    }
}
