using Microsoft.Extensions.Configuration;
using RestaurantOrders.Infrastructure.Config.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrders.Infrastructure.Config
{
    public class AppConfig
    {
        public static ConnectionStringsSettings? ConnectionStrings { get; set; }
        public static MenuDiscountSettings? MenuDiscount { get; set; }
        public static OrderDiscountSettings? OrderDiscount { get; set; }
        public static QuantityThresholdSettings? QuantityThreshold{ get; set; }

        public static void Init(IConfiguration configuration)
        {
            Configure(configuration);
        }

        private static void Configure(IConfiguration configuration)
        {
            ConnectionStrings = configuration.GetSection("ConnectionStrings").Get<ConnectionStringsSettings>();
            MenuDiscount = configuration.GetSection("MenuDiscounts").Get<MenuDiscountSettings>();
            OrderDiscount = configuration.GetSection("OrderDiscounts").Get<OrderDiscountSettings>();
            QuantityThreshold = configuration.GetSection("Restaurant").Get<QuantityThresholdSettings>();
        }
    }
}
