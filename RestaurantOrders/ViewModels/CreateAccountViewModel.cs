using RestaurantOrders.Database.Context;
using RestaurantOrders.Database.Entities;
using RestaurantOrders.Database.Enums;
using RestaurantOrders.Models;
using RestaurantOrders.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            // logica pentru adaugarea userului in DB

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Window currentWindow = Application.Current.MainWindow;
            currentWindow.Close();
            Application.Current.MainWindow = loginWindow;
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
