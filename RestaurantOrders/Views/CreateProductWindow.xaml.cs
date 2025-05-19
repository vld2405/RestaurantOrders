using Microsoft.Extensions.Configuration;
using RestaurantOrders.Infrastructure.Config;
using RestaurantOrders.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// <summary>
    /// Interaction logic for CreateProductWindow.xaml
    /// </summary>
    public partial class CreateProductWindow : Window
    {
        public event EventHandler ProductAdded;

        public CreateProductWindow()
        {
            if (AppConfig.ConnectionStrings?.RestaurantOrdersDatabase == null)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                AppConfig.Init(configuration);
            }

            InitializeComponent();

            var viewModel = (CreateProductViewModel)DataContext;
            viewModel.RequestClose += ViewModel_RequestClose;
            viewModel.ProductCreated += ViewModel_ProductCreated;
        }

        private void ViewModel_RequestClose(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ViewModel_ProductCreated(object sender, EventArgs e)
        {
            // When a product is created successfully, raise the ProductAdded event
            ProductAdded?.Invoke(this, EventArgs.Empty);
        }
    }
}
