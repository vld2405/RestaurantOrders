using RestaurantOrders.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RestaurantOrders.Views
{
    public partial class ProductInfoWindow : Window
    {
        private ProductInfoViewModel _viewModel;

        public ProductInfoWindow(int productId, bool isMenu)
        {
            InitializeComponent();

            _viewModel = new ProductInfoViewModel(productId, isMenu);
            DataContext = _viewModel;

            // Subscribe to close event
            _viewModel.RequestClose += (sender, e) => this.Close();
        }
    }
}
