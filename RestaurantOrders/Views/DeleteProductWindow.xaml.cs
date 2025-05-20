using RestaurantOrders.ViewModels;
using System;
using System.Windows;

namespace RestaurantOrders.Views
{
    /// <summary>
    /// Interaction logic for DeleteProductWindow.xaml
    /// </summary>
    public partial class DeleteProductWindow : Window
    {
        private DeleteProductViewModel _viewModel;

        public event EventHandler ProductDeleted;

        public DeleteProductWindow()
        {
            InitializeComponent();

            _viewModel = (DeleteProductViewModel)DataContext;
            _viewModel.RequestClose += ViewModel_RequestClose;
            _viewModel.ProductDeleted += ViewModel_ProductDeleted;
        }

        private void ViewModel_RequestClose(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ViewModel_ProductDeleted(object sender, EventArgs e)
        {
            ProductDeleted?.Invoke(this, EventArgs.Empty);
        }
    }
}