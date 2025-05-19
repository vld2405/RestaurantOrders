using RestaurantOrders.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrders.Models
{
    public class CategoryWithProducts
    {
        public CategoryViewModel Category { get; set; }
        public ObservableCollection<ProductItemViewModel> Products { get; set; }
    }
}
