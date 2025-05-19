using RestaurantOrders.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        private int _tempQuantity = 0; // Temporary quantity for display before adding to cart
        private bool _isMenu;

        public bool IsMenu
        {
            get => _isMenu;
            set
            {
                _isMenu = value;
                OnPropertyChanged();
            }
        }
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

        // This property is bound to the UI for display
        public int TempQuantity
        {
            get => _tempQuantity;
            set
            {
                _tempQuantity = value;
                OnPropertyChanged();
            }
        }

        public bool IsInCart => OrderQuantity > 0;

        // Event for when the product is added to cart
        public event EventHandler<EventArgs> AddedToCart;

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
            TempQuantity++;
        }

        private void DecreaseQuantity()
        {
            if (TempQuantity > 0)
                TempQuantity--;
        }

        private bool CanDecreaseQuantity() => TempQuantity > 0;

        private void AddToCart()
        {
            if (TempQuantity > 0)
            {
                OrderQuantity += TempQuantity;
                AddedToCart?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool CanAddToCart() => Quantity > 0;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}