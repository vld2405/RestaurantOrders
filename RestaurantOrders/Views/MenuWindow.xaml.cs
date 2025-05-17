using RestaurantOrders.Database.Entities;
using RestaurantOrders.Database.Enums;
using RestaurantOrders.ViewModels;
using System.Windows;

namespace RestaurantOrders.Views
{
    public partial class MenuWindow : Window
    {
        public MenuWindow(UserType userType)
        {
            InitializeComponent();
            DataContext = new MenuViewModel(userType);
        }
        public MenuWindow(User user, UserType userType)
        {
            InitializeComponent();
            DataContext = new MenuViewModel(user, userType);
        }
    }
}
