using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrders.Models
{
    public class User
    {
        public UserType UserType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
        public string? Address { get; set; }
        public string? Password { get; set; }

        public User() {}

        public User(UserType UserType, string FirstName, string LastName, string Email, string PhoneNo, string Address, string Password)
        {
            this.UserType = UserType;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.PhoneNo = PhoneNo;
            this.Address = Address;
            this.Password = Password;
        }
    }
}
