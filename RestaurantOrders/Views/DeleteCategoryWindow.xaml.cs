using RestaurantOrders.ViewModels;
using System;
using System.Windows;

namespace RestaurantOrders.Views
{
    /// <summary>
    /// Interaction logic for DeleteCategoryWindow.xaml
    /// </summary>
    public partial class DeleteCategoryWindow : Window
    {
        private DeleteCategoryViewModel _viewModel;

        public event EventHandler CategoryDeleted;

        public DeleteCategoryWindow()
        {
            InitializeComponent();

            _viewModel = (DeleteCategoryViewModel)DataContext;
            _viewModel.RequestClose += ViewModel_RequestClose;
            _viewModel.CategoryDeleted += ViewModel_CategoryDeleted;
        }

        private void ViewModel_RequestClose(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ViewModel_CategoryDeleted(object sender, EventArgs e)
        {
            CategoryDeleted?.Invoke(this, EventArgs.Empty);
        }
    }
}