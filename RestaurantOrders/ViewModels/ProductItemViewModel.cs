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
    public class ProductItemViewModel : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private decimal _price;
        private int _quantity;
        private int _categoryId;
        private int _orderQuantity = 0;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }

        public int CategoryId
        {
            get => _categoryId;
            set
            {
                _categoryId = value;
                OnPropertyChanged();
            }
        }

        public int OrderQuantity
        {
            get => _orderQuantity;
            set
            {
                _orderQuantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsInCart));
            }
        }

        public bool IsInCart => OrderQuantity > 0;

        public ICommand CommandIncreaseQuantity { get; set; }
        public ICommand CommandDecreaseQuantity { get; set; }
        public ICommand CommandAddToCart { get; set; }

        public ProductItemViewModel()
        {
            CommandIncreaseQuantity = new RelayCommand(IncreaseQuantity);
            CommandDecreaseQuantity = new RelayCommand(DecreaseQuantity, CanDecreaseQuantity);
            CommandAddToCart = new RelayCommand(AddToCart, CanAddToCart);
        }

        private void IncreaseQuantity()
        {
            OrderQuantity++;
        }

        private void DecreaseQuantity()
        {
            if (OrderQuantity > 0)
                OrderQuantity--;
        }

        private bool CanDecreaseQuantity() => OrderQuantity > 0;

        private void AddToCart()
        {
            // This will be handled by the parent view model
            if (OrderQuantity == 0)
                OrderQuantity = 1;
        }

        private bool CanAddToCart() => Quantity > 0;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
