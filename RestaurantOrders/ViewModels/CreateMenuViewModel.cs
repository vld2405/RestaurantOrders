using Microsoft.Data.SqlClient;
using RestaurantOrders.Database.Context;
using RestaurantOrders.Database.Entities;
using RestaurantOrders.Infrastructure.Config;
using RestaurantOrders.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace RestaurantOrders.ViewModels
{
    public class CreateMenuViewModel : INotifyPropertyChanged
    {
        private readonly RestaurantDbContext _dbContext;
        private bool _isClosing = false;

        // Menu properties
        private string _name = string.Empty;
        private CategoryViewModel _selectedCategory;
        private ObservableCollection<CategoryViewModel> _categories = new ObservableCollection<CategoryViewModel>();

        // Product selection properties
        private ProductViewModel _selectedProduct;
        private ObservableCollection<ProductViewModel> _availableProducts = new ObservableCollection<ProductViewModel>();
        private int _productQuantity = 1;

        // Menu products collection
        private ObservableCollection<MenuProductViewModel> _menuProducts = new ObservableCollection<MenuProductViewModel>();

        // Price calculation properties
        private decimal _totalProductsPrice = 0.0m;
        private decimal _calculatedPrice = 0.0m;
        private int _discountPercentage = 10; // Default discount percentage

        // Command properties
        public ICommand CommandSubmit { get; private set; }
        public ICommand CommandCancel { get; private set; }
        public ICommand CommandAddProduct { get; private set; }
        public ICommand CommandRemoveProduct { get; private set; }

        public CreateMenuViewModel()
        {
            _dbContext = new RestaurantDbContext();

            CommandSubmit = new RelayCommand(SubmitMenu, CanSubmitMenu);
            CommandCancel = new RelayCommand(CancelOperation);
            CommandAddProduct = new RelayCommand(AddProductToMenu, CanAddProductToMenu);
            CommandRemoveProduct = new RelayCommand<MenuProductViewModel>(RemoveProductFromMenu);

            LoadCategories();
            LoadProducts();


            if (AppConfig.Discounts?.MenuDiscount != null)
            {
                if (int.TryParse(AppConfig.Discounts.MenuDiscount, out int discount))
                {
                    _discountPercentage = discount;
                }
            }
        }

        #region Properties

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public CategoryViewModel SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        public ProductViewModel SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (_selectedProduct != value)
                {
                    _selectedProduct = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ObservableCollection<ProductViewModel> AvailableProducts
        {
            get => _availableProducts;
            set
            {
                if (_availableProducts != value)
                {
                    _availableProducts = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ProductQuantity
        {
            get => _productQuantity;
            set
            {
                if (_productQuantity != value)
                {
                    _productQuantity = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ObservableCollection<MenuProductViewModel> MenuProducts
        {
            get => _menuProducts;
            set
            {
                if (_menuProducts != value)
                {
                    _menuProducts = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal TotalProductsPrice
        {
            get => _totalProductsPrice;
            set
            {
                if (_totalProductsPrice != value)
                {
                    _totalProductsPrice = value;
                    OnPropertyChanged();
                    // Recalculate final price when total changes
                    CalculateMenuPrice();
                }
            }
        }

        public decimal CalculatedPrice
        {
            get => _calculatedPrice;
            set
            {
                if (_calculatedPrice != value)
                {
                    _calculatedPrice = value;
                    OnPropertyChanged();
                }
            }
        }

        public int DiscountPercentage
        {
            get => _discountPercentage;
            set
            {
                if (_discountPercentage != value)
                {
                    _discountPercentage = value;
                    OnPropertyChanged();
                    // Recalculate price when discount changes
                    CalculateMenuPrice();
                }
            }
        }

        public bool CanSubmit => !string.IsNullOrWhiteSpace(Name) &&
                                SelectedCategory != null &&
                                MenuProducts.Count > 0;

        #endregion

        #region Command Methods

        private bool CanSubmitMenu()
        {
            return !string.IsNullOrWhiteSpace(Name) &&
                   SelectedCategory != null &&
                   MenuProducts.Count > 0;
        }

        private void SubmitMenu()
        {
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(Name))
                {
                    MessageBox.Show("Please enter a menu name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (SelectedCategory == null)
                {
                    MessageBox.Show("Please select a category for the menu.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MenuProducts.Count == 0)
                {
                    MessageBox.Show("Please add at least one product to the menu.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Prepare data for SQL procedure
                var productIds = string.Join(",", MenuProducts.Select(p => p.Product.Id));
                var productQuantities = string.Join(",", MenuProducts.Select(p => p.Quantity));

                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    using (var command = new SqlCommand("AddMenu", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@CategoryId", SelectedCategory.Id);
                        command.Parameters.AddWithValue("@ProductIds", productIds);
                        command.Parameters.AddWithValue("@ProductQuantities", productQuantities);
                        command.Parameters.AddWithValue("@Price", DBNull.Value); // Let the procedure calculate the price

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int menuId = reader.GetInt32(reader.GetOrdinal("MenuId"));
                                string message = reader.GetString(reader.GetOrdinal("Message"));

                                if (menuId > 0)
                                {
                                    decimal calculatedPrice = 0;
                                    if (!reader.IsDBNull(reader.GetOrdinal("CalculatedPrice")))
                                    {
                                        calculatedPrice = reader.GetDecimal(reader.GetOrdinal("CalculatedPrice"));
                                    }

                                    MessageBox.Show($"Menu '{Name}' created successfully with price {calculatedPrice:F2} RON.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                                    // Raise the MenuCreated event to signal success
                                    MenuCreated?.Invoke(this, EventArgs.Empty);

                                    // Close the window
                                    OnRequestClose();
                                }
                                else
                                {
                                    MessageBox.Show($"Failed to create menu: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelOperation()
        {
            OnRequestClose();
        }

        private bool CanAddProductToMenu()
        {
            return SelectedProduct != null && ProductQuantity > 0;
        }

        private void AddProductToMenu()
        {
            if (SelectedProduct == null || ProductQuantity <= 0)
                return;

            // Check if the product is already in the menu
            var existingProduct = MenuProducts.FirstOrDefault(p => p.Product.Id == SelectedProduct.Id);
            if (existingProduct != null)
            {
                // Update quantity if already exists
                existingProduct.Quantity += ProductQuantity;
                // Refresh UI binding
                MenuProducts.Remove(existingProduct);
                MenuProducts.Add(existingProduct);
            }
            else
            {
                // Add new product to menu
                var menuProduct = new MenuProductViewModel
                {
                    Product = SelectedProduct,
                    Quantity = ProductQuantity
                };
                MenuProducts.Add(menuProduct);
            }

            // Reset selection for next product
            ProductQuantity = 1;
            SelectedProduct = null;

            // Recalculate total price
            CalculateTotalPrice();

            // Update commands state
            CommandManager.InvalidateRequerySuggested();
        }

        private void RemoveProductFromMenu(MenuProductViewModel product)
        {
            if (product != null)
            {
                MenuProducts.Remove(product);

                // Recalculate total price
                CalculateTotalPrice();

                // Update commands state
                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion

        #region Helper Methods

        private void CalculateTotalPrice()
        {
            TotalProductsPrice = MenuProducts.Sum(p => p.TotalPrice);
        }

        private void CalculateMenuPrice()
        {
            // Apply discount to the total price
            CalculatedPrice = Math.Round(TotalProductsPrice * (1 - (DiscountPercentage / 100.0m)), 2);
        }

        private void LoadCategories()
        {
            if (_isClosing) return;

            try
            {
                Categories.Clear();

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

                // Pre-select the Menu category if available
                var menuCategory = Categories.FirstOrDefault(c => c.Name == "Menu");
                if (menuCategory != null)
                {
                    SelectedCategory = menuCategory;
                }
            }
            catch (Exception ex)
            {
                if (!_isClosing)
                    MessageBox.Show($"Failed to load categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProducts()
        {
            if (_isClosing) return;

            try
            {
                AvailableProducts.Clear();

                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    // Get all products except those in the Menu category
                    using (var command = new SqlCommand(@"
                        SELECT p.Id, p.Name, p.Price, p.Quantity, p.CategoryId, c.Name as CategoryName 
                        FROM Products p
                        JOIN Categories c ON p.CategoryId = c.Id
                        WHERE c.Name <> 'Menu'
                        ORDER BY c.Name, p.Name", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var product = new ProductViewModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                    CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                    CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
                                };

                                AvailableProducts.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!_isClosing)
                    MessageBox.Show($"Failed to load products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Events and Cleanup

        // Event that signals a menu was successfully created
        public event EventHandler MenuCreated;

        // Event that signals the window should close
        public event EventHandler RequestClose;

        protected virtual void OnRequestClose()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            _isClosing = true;
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
    
}