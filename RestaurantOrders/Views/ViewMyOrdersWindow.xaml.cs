using RestaurantOrders.Database.Entities;
using RestaurantOrders.ViewModels;
using System;
using System.Windows;

namespace RestaurantOrders.Views
{
    /// <summary>
    /// Interaction logic for ViewMyOrdersWindow.xaml
    /// </summary>
    public partial class ViewMyOrdersWindow : Window
    {
        private ViewMyOrdersViewModel _viewModel;

        public ViewMyOrdersWindow(User user)
        {
            InitializeComponent();

            // Initialize the view model with the current user
            _viewModel = new ViewMyOrdersViewModel(user);
            DataContext = _viewModel;

            // Subscribe to the RequestClose event
            _viewModel.RequestClose += (sender, e) => Close();
        }
    }
}