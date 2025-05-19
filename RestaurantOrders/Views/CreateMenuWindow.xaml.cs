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
    /// Interaction logic for CreateMenuWindow.xaml
    /// </summary>
    public partial class CreateMenuWindow : Window
    {
        public event EventHandler MenuAdded;

        public CreateMenuWindow()
        {
            InitializeComponent();

            var viewModel = (CreateMenuViewModel)DataContext;
            viewModel.RequestClose += (sender, e) => this.Close();
            viewModel.MenuCreated += ViewModel_MenuCreated;
        }

        private void ViewModel_MenuCreated(object sender, EventArgs e)
        {
            // When a menu is created successfully, raise the MenuAdded event
            MenuAdded?.Invoke(this, EventArgs.Empty);
        }
    }
}