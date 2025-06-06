﻿using RestaurantOrders.ViewModels;
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
    public partial class CreateAccountWindow : Window
    {
        public CreateAccountWindow()
        {
            InitializeComponent();
        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (CreateAccountViewModel)DataContext;

            // Get password from PasswordBox (can't bind directly for security reasons)
            viewModel.Password = PasswordBox.Password;

            // Verify passwords match
            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Passwords do not match.", "Password Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
    }
}
