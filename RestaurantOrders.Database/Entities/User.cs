using RestaurantOrders.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrders.Database.Entities
{
    public class User
    {
        public int Id { get; set; }
        public UserType UserType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
        public string? Address { get; set; }
        public string? Password { get; set; }

        public ICollection<Order> Orders { get; set; } = null!;
    }
}
