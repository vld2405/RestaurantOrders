using RestaurantOrders.Database.Entities;
using RestaurantOrders.Database.Enums;
using RestaurantOrders.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestaurantOrders.ViewModels
{
    public class MenuViewModel : INotifyPropertyChanged
    {
        public MenuViewModel()
        {
            CommandAddProduct = new RelayCommand(AddProduct);
            AdminMenuButton = new RelayCommand(OpenAdminMenuButton);
        }
        public MenuViewModel(UserType userType)
        {
            if(userType == UserType.NoAccount)
            {
                IsSubmitEnabled = false;
                IsAdminVisibility = false;
            }
        }
        public MenuViewModel(User user, UserType userType)
        {
            if (userType == UserType.NoAccount)
            {
                IsSubmitEnabled = false;
                IsAdminVisibility = false;
            }
        }

        private bool _isSubmitEnabled = true;
        private bool _isAdminVisibility = true;

        #region getters-setters
        public bool IsSubmitEnabled 
        { 
            get
            {
                return _isSubmitEnabled;
            }

            set
            {
                _isSubmitEnabled = value;
                OnPropertyChanged(nameof(IsSubmitEnabled));
            }
        }
        public bool IsAdminVisibility
        {
            get
            {
                return _isAdminVisibility;
            }

            set
            {
                _isAdminVisibility = value;
                OnPropertyChanged(nameof(IsAdminVisibility));
            }
        }
        #endregion

        #region Command-Declarations
        public ICommand CommandAddProduct { get; set; }
        public ICommand AdminMenuButton { get; set; }

        #endregion

        #region Command-Methods

        private void AddProduct()
        {

        }

        private void OpenAdminMenuButton()
        {

        }

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
