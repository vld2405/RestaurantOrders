using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RestaurantOrders.Database.Context;
using RestaurantOrders.Database.Entities;
using RestaurantOrders.Infrastructure.Config;
using RestaurantOrders.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace RestaurantOrders.ViewModels
{
    public class CreateProductViewModel : INotifyPropertyChanged
    {
        public CreateProductViewModel()
        {
            _dbContext = new RestaurantDbContext();
            LoadCategories();
            LoadAllergens();

            CommandSubmit = new RelayCommand(SubmitButton);
            CommandCancel = new RelayCommand(CancelButton);
        }

        private string _name = string.Empty;
        private Category _selectedCategory;
        private decimal _price;
        private int _quantity = 1;
        private ObservableCollection<CategoryViewModel> _categories = new ObservableCollection<CategoryViewModel>();
        private ObservableCollection<AllergenViewModel> _availableAllergens = new ObservableCollection<AllergenViewModel>();

        private bool _isClosing = false;

        #region getter-setter

        public string Name 
        {
            get => _name;

            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));
                }
            }
        }
        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged(nameof(Price));
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
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }
        public ObservableCollection<CategoryViewModel> Categories
        {
            get => _categories;
            set
            {
                if (_categories != value)
                {
                    _categories = value;
                    OnPropertyChanged(nameof(Categories));
                }
            }
        }
        public ObservableCollection<AllergenViewModel> AvailableAllergens
        {
            get => _availableAllergens;
            set
            {
                if (_availableAllergens != value)
                {
                    _availableAllergens = value;
                    OnPropertyChanged(nameof(Allergen));
                }
            }
        }

        #endregion

        #region Command-Declarations
        public ICommand CommandSubmit { get; set; }
        public ICommand CommandCancel { get; set; }

        #endregion

        #region Command-Methods
        private void SubmitButton()
        {

        }
        private void CancelButton()
        {
            OnRequestClose();
        }

        #endregion

        public event EventHandler? RequestClose;

        protected virtual void OnRequestClose()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        #region Load-Collections

        private readonly RestaurantDbContext _dbContext;

        private void LoadCategories()
        {
            if (_isClosing) return;

            try
            {
                Categories = new ObservableCollection<CategoryViewModel>();

                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    using (var command = new SqlCommand("GetAllCategories", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var category = new CategoryViewModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name"))
                                };

                                Categories.Add(category);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!_isClosing)
                    MessageBox.Show($"Failed to load categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAllergens()
        {
            if (_isClosing) return;

            try
            {
                AvailableAllergens.Clear();

                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();
                    using (var command = new SqlCommand("GetAllAllergens", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var allergen = new AllergenViewModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    IsSelected = false
                                };

                                AvailableAllergens.Add(allergen);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!_isClosing)
                    MessageBox.Show($"Failed to load allergens: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Dispose()
        {
            _isClosing = true;
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
