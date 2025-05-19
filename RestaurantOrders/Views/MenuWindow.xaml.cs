using RestaurantOrders.Database.Entities;
using RestaurantOrders.Database.Enums;
using RestaurantOrders.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RestaurantOrders.Views
{
    public partial class MenuWindow : Window
    {
        private MenuViewModel _viewModel;

        public MenuWindow(UserType userType)
        {
            InitializeComponent();
            _viewModel = new MenuViewModel(userType);
            DataContext = _viewModel;
        }

        public MenuWindow(User user, UserType userType)
        {
            InitializeComponent();
            _viewModel = new MenuViewModel(user, userType);
            DataContext = _viewModel;
        }

        private void AdminMenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the context menu when the button is clicked
            Button button = (Button)sender;
            if (button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
                e.Handled = true;
            }
        }
        private void AddMenu()
        {
            CreateMenuWindow createMenuWindow = new CreateMenuWindow();
            createMenuWindow.Owner = this;

            // Add this line to subscribe to the MenuAdded event
            createMenuWindow.MenuAdded += CreateMenuWindow_MenuAdded;

            createMenuWindow.ShowDialog();
        }

        // Add this event handler method to MenuWindow.xaml.cs
        private void CreateMenuWindow_MenuAdded(object sender, EventArgs e)
        {
            // Refresh the menu data
            RefreshMenu();
        }
        public void RefreshMenu()
        {
            if (_viewModel != null)
            {
                _viewModel.RefreshCategoriesAndProducts();
            }
        }
    }
}