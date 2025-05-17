using Azure.Identity;
using RestaurantOrders.Database.Enums;
using RestaurantOrders.Database.Enums;
using RestaurantOrders.Models;
using RestaurantOrders.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace RestaurantOrders.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        //TODO 1: Logica pentru creat useri si tot. + sa vad cum fac cu baza de date
        public LoginViewModel()
        {
            CommandLoginButton = new RelayCommand(LoginButtonClicked);
            CommandCreateAccountButton = new RelayCommand(CreateAccountClicked);
            CommandNoAccountEnteringButton = new RelayCommand(NoAccountEnteringClicked);
        }

        private string _email = string.Empty;
        private string _password = string.Empty;

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
                    OnPropertyChanged();
                }
            }
        }

        #region CommandMethods

        public void LoginButtonClicked()
        {
            // TODO : Logica pentru logare
        }
        
        public void CreateAccountClicked()
        {
            CreateAccountWindow accountWindow = new CreateAccountWindow();
            accountWindow.Show();
            Window currentWindow = Application.Current.MainWindow;
            currentWindow.Close();
            Application.Current.MainWindow = accountWindow;
        }
        
        public void NoAccountEnteringClicked()
        {
            MenuWindow menuWindow = new MenuWindow();
            menuWindow.Show();
            Window currentWindow = Application.Current.MainWindow;
            currentWindow.Close();
            Application.Current.MainWindow = menuWindow;
        }

        #endregion

        #region Command Declaration

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
