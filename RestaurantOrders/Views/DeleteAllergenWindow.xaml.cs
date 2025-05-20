using RestaurantOrders.ViewModels;
using System;
using System.Windows;

namespace RestaurantOrders.Views
{
    /// <summary>
    /// Interaction logic for DeleteAllergenWindow.xaml
    /// </summary>
    public partial class DeleteAllergenWindow : Window
    {
        private DeleteAllergenViewModel _viewModel;

        public event EventHandler AllergenDeleted;

        public DeleteAllergenWindow()
        {
            InitializeComponent();

            _viewModel = (DeleteAllergenViewModel)DataContext;
            _viewModel.RequestClose += ViewModel_RequestClose;
            _viewModel.AllergenDeleted += ViewModel_AllergenDeleted;
        }

        private void ViewModel_RequestClose(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ViewModel_AllergenDeleted(object sender, EventArgs e)
        {
            AllergenDeleted?.Invoke(this, EventArgs.Empty);
        }
    }
}