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

        #region Commands

        public ICommand CommandLoginButton { get; set; }
        public ICommand CommandNoAccountEnteringButton { get; set; }

        #endregion

        #region CommandMethods

        public void LoginButtonClicked()
        {

        }
        
        public void NoAccountEnteringClicked()
        {
            MenuWindow menuWindow = new MenuWindow();
            menuWindow.Show();
            Window loginWindow = Application.Current.MainWindow;
            loginWindow.Close();
            Application.Current.MainWindow = menuWindow;
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
