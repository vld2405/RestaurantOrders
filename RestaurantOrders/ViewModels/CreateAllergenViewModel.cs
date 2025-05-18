using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrders.ViewModels
{
    public class CreateAllergenViewModel : INotifyPropertyChanged
    {
        private string _allergenName = "";
        private bool _canSubmit = false;

        public bool CanSubmit 
        { 
            get {  return _canSubmit; }
            
            set
            {
                _canSubmit = value;
                OnPropertyChanged(nameof(CanSubmit));
            }
        }

        public string AllergenName
        {
            get { return _allergenName; }

            set
            {
                _allergenName = value;

                if (_allergenName != "")
                    CanSubmit = true;
                else
                    CanSubmit = false;

                OnPropertyChanged(nameof(AllergenName));
            }
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
