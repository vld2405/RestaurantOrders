using RestaurantOrders.ViewModels;
using System;
using System.Windows;

namespace RestaurantOrders.Views
{
    /// <summary>
    /// Interaction logic for ViewOrdersWindow.xaml
    /// </summary>
    public partial class ViewOrdersWindow : Window
    {
        private ViewOrdersViewModel _viewModel;

        public ViewOrdersWindow()
        {
            InitializeComponent();
            _viewModel = new ViewOrdersViewModel();
            DataContext = _viewModel;

            // Subscribe to the RequestClose event
            _viewModel.RequestClose += (sender, e) => Close();
        }
    }
}