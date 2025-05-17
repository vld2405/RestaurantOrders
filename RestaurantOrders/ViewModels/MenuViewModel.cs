using RestaurantOrders.Database.Entities;
using RestaurantOrders.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrders.ViewModels
{
    public class MenuViewModel : INotifyPropertyChanged
    {
        private bool _isSubmitEnabled = true;
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

        public MenuViewModel(UserType userType)
        {
            if(userType == UserType.NoAccount)
                IsSubmitEnabled = false;
        }
        
        public MenuViewModel(User user, UserType userType)
        {
            if (userType == UserType.NoAccount)
                IsSubmitEnabled = false;
        }

        #region Property Changed

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
