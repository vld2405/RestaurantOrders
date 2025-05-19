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
        public CreateProductWindow()
        {
            InitializeComponent();

            //if (AppConfig.ConnectionStrings?.RestaurantOrdersDatabase == null)
            //{
            //    var configuration = new ConfigurationBuilder()
            //        .SetBasePath(Directory.GetCurrentDirectory())
            //        .AddJsonFile("appsettings.json", optional: false)
            //        .Build();

            //    AppConfig.Init(configuration);
            //}

            var viewModel = (CreateProductViewModel)DataContext;
            viewModel.RequestClose += (sender, e) => this.Close();
        }
    }
}
