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
    /// <summary>
    /// Interaction logic for CreateAllergenWindow.xaml
    /// </summary>
    public partial class CreateAllergenWindow : Window
    {
        public CreateAllergenWindow()
        {
            InitializeComponent();

            var viewModel = (CreateAllergenViewModel)DataContext;
            viewModel.RequestClose += (sender, e) => this.Close();
        }
    }
}
