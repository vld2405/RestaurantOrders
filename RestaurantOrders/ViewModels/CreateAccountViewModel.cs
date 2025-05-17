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
            // TODO: Logica pentru adaugat useri in baza de date

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
