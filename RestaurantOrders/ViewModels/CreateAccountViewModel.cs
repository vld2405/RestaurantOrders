using Microsoft.Data.SqlClient;
using RestaurantOrders.Database.Context;
using RestaurantOrders.Database.Entities;
using RestaurantOrders.Database.Enums;
using RestaurantOrders.Infrastructure.Config;
using RestaurantOrders.Models;
using RestaurantOrders.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RestaurantOrders.ViewModels
{
    public class CreateAccountViewModel
    {
        public CreateAccountViewModel()
        {
            CommandCancelButton = new RelayCommand(CancelClicked);
            CommandCreateAccount = new RelayCommand(CreateAccountClicked);
        }

        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phoneNo;
        private string _address;
        private string _password;

        #region getters-setters
        public string FirstName
        {
            get { return _firstName; }

            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged(nameof(FirstName));
                }
            }
        }
        public string LastName
        {
            get { return _lastName; }

            set
            {
                if(_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged(nameof(LastName));
                }
            }
        }

        public string Email
        {
            get { return _email; }

            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string PhoneNo
        {
            get { return _phoneNo; }

            set
            {
                if (_phoneNo != value)
                {
                    _phoneNo = value;
                    OnPropertyChanged(nameof(PhoneNo));
                }
            }
        }

        public string Address
        {
            get { return _address; }

            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged(nameof(Address));
                }
            }
        }

        public string Password
        {
            get { return _password; }

            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        #endregion

        #region Command Methods
        public void CancelClicked()
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Window currentWindow = Application.Current.MainWindow;
            currentWindow.Close();
            Application.Current.MainWindow = loginWindow;
        }
        
        public void CreateAccountClicked()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FirstName) ||
                    string.IsNullOrWhiteSpace(LastName) ||
                    string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(Password))
                {
                    MessageBox.Show("Please fill in all required fields.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    using (var command = new SqlCommand("AddUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@FirstName", FirstName);
                        command.Parameters.AddWithValue("@LastName", LastName);
                        command.Parameters.AddWithValue("@Email", Email);
                        command.Parameters.AddWithValue("@PhoneNo", PhoneNo ?? string.Empty);
                        command.Parameters.AddWithValue("@Address", Address ?? string.Empty);
                        command.Parameters.AddWithValue("@Password", Password); // Consider hashing

                        if (Email.Contains("@admin."))
                            command.Parameters.AddWithValue("@UserType", (int)UserType.Employee);
                        else
                            command.Parameters.AddWithValue("@UserType", (int)UserType.Client);

                        var result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            int userId = Convert.ToInt32(result);

                            if (userId == -1)
                            {
                                MessageBox.Show("A user with this email already exists.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            //MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            LoginWindow loginWindow = new LoginWindow();
                            loginWindow.Show();
                            Window currentWindow = Application.Current.MainWindow;
                            currentWindow.Close();
                            Application.Current.MainWindow = loginWindow;
                        }
                        else
                        {
                            MessageBox.Show("Failed to create account. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Command Declaration
        public ICommand CommandCancelButton { get; set; }
        public ICommand CommandCreateAccount { get; set; }

        #endregion

        #region Property Changed
        
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
