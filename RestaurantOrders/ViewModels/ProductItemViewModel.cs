using RestaurantOrders.Models;
using RestaurantOrders.Views;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
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
        private string _categoryName;
        private int _orderQuantity = 0;
        private int _tempQuantity = 0;
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

        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged();
            }
        }

        public int OrderQuantity
        {
            get => _orderQuantity;
            set
            {
                _orderQuantity = Math.Max(0, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsInCart));
            }
        }

        public int TempQuantity
        {
            get => _tempQuantity;
            set
            {
                _tempQuantity = Math.Max(0, value);
                OnPropertyChanged();
            }
        }

        public bool IsInCart => OrderQuantity > 0;

        public event EventHandler<EventArgs> AddedToCart;

        public ICommand CommandIncreaseQuantity { get; set; }
        public ICommand CommandDecreaseQuantity { get; set; }
        public ICommand CommandAddToCart { get; set; }
        public ICommand CommandInfoButton { get; set; }
        public ICommand CommandRemoveFromCart { get; set; }

        public ProductItemViewModel()
        {
            CommandIncreaseQuantity = new RelayCommand(IncreaseQuantity);
            CommandDecreaseQuantity = new RelayCommand(DecreaseQuantity, CanDecreaseQuantity);
            CommandAddToCart = new RelayCommand(AddToCart, CanAddToCart);
            CommandInfoButton = new RelayCommand(OpenInfo);
            CommandRemoveFromCart = new RelayCommand(RemoveFromCart);
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
                TempQuantity = 0;
                AddedToCart?.Invoke(this, EventArgs.Empty);
            }
        }

        private void RemoveFromCart()
        {
            OrderQuantity = 0;
            AddedToCart?.Invoke(this, EventArgs.Empty);
        }

        private void OpenInfo()
        {
            try
            {
                ProductInfoWindow productInfoWindow = new ProductInfoWindow(Id, IsMenu);
                productInfoWindow.Owner = Application.Current.MainWindow;
                productInfoWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening product info: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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