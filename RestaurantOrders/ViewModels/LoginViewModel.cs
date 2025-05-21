using Azure.Identity;
using Microsoft.Data.SqlClient;
using RestaurantOrders.Database.Entities;
using RestaurantOrders.Database.Enums;
using RestaurantOrders.Infrastructure.Config;
using RestaurantOrders.Models;
using RestaurantOrders.Views;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace RestaurantOrders.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public LoginViewModel()
        {
            CommandLoginButton = new RelayCommand(LoginButtonClicked);
            CommandCreateAccountButton = new RelayCommand(CreateAccountClicked);
            CommandNoAccountEnteringButton = new RelayCommand(NoAccountEnteringClicked);
        }

        private string _email = string.Empty;
        private string _password = string.Empty;

        #region getters-setters

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get => _password;
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

        #region CommandMethods

        private void LoginButtonClicked()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    MessageBox.Show("Please enter both email and password.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                User? user = VerifyUserCredentials(Email, Password);

                if (user != null)
                {

                    MenuWindow menuWindow = new MenuWindow(user, user.UserType);
                    menuWindow.Show();

                    Window currentWindow = Application.Current.MainWindow;
                    currentWindow.Close();
                    Application.Current.MainWindow = menuWindow;
                }
                else
                {
                    MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"An error occurred: {ex.Message}", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private User? VerifyUserCredentials(string email, string password)
        {
            User? user = null;

            try
            {
                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    using (var command = new SqlCommand("VerifyUserCredentials", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new User
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    UserType = (UserType)reader.GetInt32(reader.GetOrdinal("UserType")),
                                    FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    PhoneNo = reader.IsDBNull(reader.GetOrdinal("PhoneNo")) ? null : reader.GetString(reader.GetOrdinal("PhoneNo")),
                                    Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? null : reader.GetString(reader.GetOrdinal("Address"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in VerifyUserCredentials: {ex.Message}");
                throw;
            }

            return user;
        }

        private void CreateAccountClicked()
        {
            CreateAccountWindow accountWindow = new CreateAccountWindow();
            accountWindow.Show();
            Window currentWindow = Application.Current.MainWindow;
            currentWindow.Close();
            Application.Current.MainWindow = accountWindow;
        }
        
        private void NoAccountEnteringClicked()
        {
            MenuWindow menuWindow = new MenuWindow(UserType.NoAccount);
            menuWindow.Show();
            Window currentWindow = Application.Current.MainWindow;
            currentWindow.Close();
            Application.Current.MainWindow = menuWindow;
        }

        #endregion

        #region Command-Declaration

        public ICommand CommandLoginButton { get; set; }
        public ICommand CommandCreateAccountButton { get; set; }
        public ICommand CommandNoAccountEnteringButton { get; set; }

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
