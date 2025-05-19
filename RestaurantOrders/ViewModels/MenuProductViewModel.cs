using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestaurantOrders.ViewModels
{
    public class MenuProductViewModel : INotifyPropertyChanged
    {
        private ProductViewModel _product;
        private int _quantity;

        public ProductViewModel Product
        {
            get => _product;
            set
            {
                if (_product != value)
                {
                    _product = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPrice));
                    OnPropertyChanged(nameof(WeightRatio));
                }
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPrice));
                    OnPropertyChanged(nameof(WeightRatio));
                }
            }
        }

        public decimal WeightRatio => Product?.Quantity > 0 ? (decimal)Quantity / Product.Quantity : 0m;

        public decimal TotalPrice => Product != null ? Product.Price * WeightRatio : 0m;

        public string WeightRatioText => Product?.Quantity > 0
            ? $"{Quantity}g / {Product.Quantity}g = {WeightRatio:F2}"
            : string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}