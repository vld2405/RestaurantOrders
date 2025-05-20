using RestaurantOrders.ViewModels;
using System;
using System.Windows;

namespace RestaurantOrders.Views
{
    /// <summary>
    /// Interaction logic for DeleteMenuWindow.xaml
    /// </summary>
    public partial class DeleteMenuWindow : Window
    {
        private DeleteMenuViewModel _viewModel;

        public event EventHandler MenuDeleted;

        public DeleteMenuWindow()
        {
            InitializeComponent();

            _viewModel = (DeleteMenuViewModel)DataContext;
            _viewModel.RequestClose += ViewModel_RequestClose;
            _viewModel.MenuDeleted += ViewModel_MenuDeleted;
        }

        private void ViewModel_RequestClose(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ViewModel_MenuDeleted(object sender, EventArgs e)
        {
            MenuDeleted?.Invoke(this, EventArgs.Empty);
        }
    }
}