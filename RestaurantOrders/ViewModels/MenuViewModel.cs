using Microsoft.Data.SqlClient;
using RestaurantOrders.Database.Entities;
using RestaurantOrders.Database.Enums;
using RestaurantOrders.Infrastructure.Config;
using RestaurantOrders.Models;
using RestaurantOrders.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RestaurantOrders.ViewModels
{
    public class MenuViewModel : INotifyPropertyChanged
    {
        public MenuViewModel()
        {
            CommandAddProduct = new RelayCommand(AddProduct);
            CommandAddAllergen = new RelayCommand(AddAllergen);
            CommandAddCategory = new RelayCommand(AddCategory);
            CommandCreateMenu = new RelayCommand(CreateMenu);
            CommandDeleteProduct = new RelayCommand(DeleteProduct);
            CommandDeleteAllergen = new RelayCommand(DeleteAllergen);
            CommandDeleteMenu = new RelayCommand(DeleteMenu);
            CommandDeleteCategory = new RelayCommand(DeleteCategory);
            CommandPlaceOrder = new RelayCommand(PlaceOrder, CanPlaceOrder);
            CommandLoginLogout = new RelayCommand(LoginLogoutButton);


            CategoriesWithProducts = new ObservableCollection<CategoryWithProducts>();
            FilteredCategoriesWithProducts = CategoriesWithProducts; // Initialize with full list
            CartItems = new ObservableCollection<ProductItemViewModel>();

            LoadCategoriesAndProducts();
        }
        public MenuViewModel(UserType userType) : this()
        {
            if (userType == UserType.NoAccount)
            {
                IsSubmitEnabled = false;
                IsAdminVisibility = false;
                LoginLogoutText = "Log in";
            }
            if (userType == UserType.Client)
            {
                IsAdminVisibility = false;
                LoginLogoutText = "Log out";
            }
        }

        public MenuViewModel(User user, UserType userType) : this(userType)
        {
            CurrentUser = user;
            LoginLogoutText = "Log out";
        }

        private bool _isSubmitEnabled = true;
        private bool _isAdminVisibility = true;
        private string _loginLogoutText = "";

        private ObservableCollection<CategoryWithProducts> _categoriesWithProducts;
        private decimal _cartTotal;
        private ObservableCollection<ProductItemViewModel> _cartItems;
        private string _searchTerm = string.Empty;
        private ObservableCollection<CategoryWithProducts> _filteredCategoriesWithProducts;

        private User _currentUser;

        #region getters-setters

        public string LoginLogoutText
        {
            get => _loginLogoutText;
            set
            {
                _loginLogoutText = value;
                OnPropertyChanged();
            }
        }
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CategoryWithProducts> CategoriesWithProducts
        {
            get => _categoriesWithProducts;
            set
            {
                _categoriesWithProducts = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CategoryWithProducts> FilteredCategoriesWithProducts
        {
            get => _filteredCategoriesWithProducts;
            set
            {
                _filteredCategoriesWithProducts = value;
                OnPropertyChanged();
            }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (_searchTerm != value)
                {
                    _searchTerm = value;
                    OnPropertyChanged();
                    FilterProducts();
                }
            }
        }

        public decimal CartTotal
        {
            get => _cartTotal;
            set
            {
                _cartTotal = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProductItemViewModel> CartItems
        {
            get => _cartItems;
            set
            {
                _cartItems = value;
                OnPropertyChanged();
            }
        }

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
        public ICommand CommandAddAllergen { get; set; }
        public ICommand CommandCreateMenu { get; set; }
        public ICommand CommandDeleteProduct { get; set; }
        public ICommand CommandDeleteAllergen { get; set; }
        public ICommand CommandDeleteMenu { get; set; }
        public ICommand CommandAddCategory { get; set; }
        public ICommand CommandDeleteCategory { get; set; }
        public ICommand CommandPlaceOrder { get; set; }
        public ICommand CommandLoginLogout { get; set; }

        #endregion

        #region Command-Methods

        private void AddProduct()
        {
            CreateProductWindow createProductWindow = new CreateProductWindow();
            createProductWindow.Owner = Application.Current.MainWindow;

            // Subscribe to the ProductAdded event to refresh the menu when a product is added
            createProductWindow.ProductAdded += CreateProductWindow_ProductAdded;

            createProductWindow.ShowDialog();
        }

        private void CreateProductWindow_ProductAdded(object sender, EventArgs e)
        {
            // Refresh the menu when a product is added
            RefreshCategoriesAndProducts();
        }

        private void AddAllergen()
        {
            CreateAllergenWindow createAllergenWindow = new CreateAllergenWindow();
            createAllergenWindow.Owner = Application.Current.MainWindow;
            createAllergenWindow.ShowDialog();
        }
        private void CreateMenu()
        {
            CreateMenuWindow createMenuWindow = new CreateMenuWindow();
            createMenuWindow.Owner = Application.Current.MainWindow;

            createMenuWindow.MenuAdded += CreateMenuWindow_MenuAdded;

            createMenuWindow.ShowDialog();
        }

        private void CreateMenuWindow_MenuAdded(object sender, EventArgs e)
        {
            RefreshCategoriesAndProducts();
        }

        private void DeleteProduct()
        {

        }
        private void DeleteAllergen()
        {

        }
        private void DeleteCategory()
        {

        }
        private void DeleteMenu()
        {

        }
        private void LoginLogoutButton()
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Window currentWindow = Application.Current.MainWindow;
            currentWindow.Close();
            Application.Current.MainWindow = loginWindow;
        }
        private void AddCategory()
        {
            CreateCategoryWindow createCategoryWindow = new CreateCategoryWindow();
            createCategoryWindow.Owner = Application.Current.MainWindow;
            createCategoryWindow.ShowDialog();
        }

        // Event handler for when a product is added to cart
        private void Product_AddedToCart(object sender, EventArgs e)
        {
            UpdateCart();
        }

        private void UpdateCart()
        {
            // Update cart items
            CartItems.Clear();
            foreach (var category in CategoriesWithProducts)
            {
                foreach (var product in category.Products)
                {
                    if (product.OrderQuantity > 0)
                    {
                        CartItems.Add(product);
                    }
                }
            }

            // Calculate total
            CartTotal = CartItems.Sum(p => p.Price * p.OrderQuantity);

            // Update commands
            CommandManager.InvalidateRequerySuggested();
        }

        private bool CanPlaceOrder()
        {
            return IsSubmitEnabled && CartItems.Any();
        }

        private void PlaceOrder()
        {
            try
            {
                // Check if there are items in the cart
                if (!CartItems.Any())
                {
                    MessageBox.Show("Your cart is empty. Please add items to your order.", "Empty Cart", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Create lists to store products and menus separately
                List<int> productIds = new List<int>();
                List<int> menuIds = new List<int>();
                List<int> quantities = new List<int>();

                // Separate products and menus for the SQL procedure
                foreach (var item in CartItems)
                {
                    if (item.IsMenu)
                    {
                        menuIds.Add(item.Id);
                        quantities.Add(item.OrderQuantity);
                    }
                    else
                    {
                        productIds.Add(item.Id);
                        quantities.Add(item.OrderQuantity);
                    }
                }

                // Convert to comma-separated strings for SQL
                string productIdsStr = string.Join(",", productIds);
                string menuIdsStr = string.Join(",", menuIds);
                string quantitiesStr = string.Join(",", quantities);

                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    using (var command = new SqlCommand("AddOrder", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Check if using a logged-in user or anonymous user
                        int userId = 1; // Default to user ID 1 for anonymous or testing

                        // Get actual user ID if available
                        if (CurrentUser != null && CurrentUser.Id > 0)
                        {
                            userId = CurrentUser.Id;
                        }

                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@OrderState", (int)OrderState.Registered);
                        command.Parameters.AddWithValue("@ProductIds", productIdsStr);

                        // Only add MenuIds parameter if there are menus in the order
                        if (menuIds.Count > 0)
                            command.Parameters.AddWithValue("@MenuIds", menuIdsStr);
                        else
                            command.Parameters.AddWithValue("@MenuIds", DBNull.Value);

                        command.Parameters.AddWithValue("@Quantities", quantitiesStr);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int orderId = reader.GetInt32(reader.GetOrdinal("OrderId"));
                                string message = reader.GetString(reader.GetOrdinal("Message"));

                                if (orderId > 0)
                                {
                                    // Get the estimated delivery time from the result
                                    DateTime estimatedDelivery = reader.GetDateTime(reader.GetOrdinal("EstimatedDeliveryTime"));

                                    // Format the time for display
                                    string deliveryTimeStr = estimatedDelivery.ToString("hh:mm tt");

                                    MessageBox.Show($"Your order #{orderId} has been placed successfully!\n\nEstimated delivery time: {deliveryTimeStr}",
                                        "Order Placed", MessageBoxButton.OK, MessageBoxImage.Information);

                                    // Reset cart
                                    foreach (var product in CartItems)
                                    {
                                        product.OrderQuantity = 0;
                                        product.TempQuantity = 0;
                                    }

                                    UpdateCart();
                                }
                                else
                                {
                                    MessageBox.Show($"Failed to place order: {message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Search Functionality

        // Filter products based on search term
        private void FilterProducts()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                FilteredCategoriesWithProducts = CategoriesWithProducts;
                return;
            }

            var filtered = new ObservableCollection<CategoryWithProducts>();

            foreach (var category in CategoriesWithProducts)
            {
                var filteredProducts = new ObservableCollection<ProductItemViewModel>(
                    category.Products.Where(p => p.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                );

                if (filteredProducts.Any())
                {
                    filtered.Add(new CategoryWithProducts
                    {
                        Category = category.Category,
                        Products = filteredProducts
                    });
                }
            }

            FilteredCategoriesWithProducts = filtered;
        }

        #endregion

        // Public method to refresh the categories and products
        public void RefreshCategoriesAndProducts()
        {
            // Clear existing event handlers to prevent memory leaks
            if (CategoriesWithProducts != null)
            {
                foreach (var category in CategoriesWithProducts)
                {
                    if (category.Products != null)
                    {
                        foreach (var product in category.Products)
                        {
                            product.AddedToCart -= Product_AddedToCart;
                        }
                    }
                }
            }

            // Clear existing collections
            CategoriesWithProducts.Clear();
            CartItems.Clear();
            CartTotal = 0;

            // Reload all categories and products
            LoadCategoriesAndProducts();

            // Apply current search filter
            FilterProducts();
        }

        private void LoadCategoriesAndProducts()
        {
            try
            {
                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    // Load categories
                    var categories = new List<CategoryViewModel>();
                    using (var command = new SqlCommand("GetAllCategories", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categories.Add(new CategoryViewModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name"))
                                });
                            }
                        }
                    }

                    // For each category, load products
                    foreach (var category in categories)
                    {
                        var productsInCategory = new List<ProductItemViewModel>();

                        // Load regular products
                        using (var command = new SqlCommand(
                            @"SELECT p.Id, p.Name, p.Quantity, p.Price, p.CategoryId, c.Name as CategoryName 
                      FROM Products p 
                      JOIN Categories c ON p.CategoryId = c.Id 
                      WHERE p.CategoryId = @CategoryId", connection))
                        {
                            command.Parameters.AddWithValue("@CategoryId", category.Id);
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var product = new ProductItemViewModel
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                        Name = reader.GetString(reader.GetOrdinal("Name")),
                                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                        CategoryId = category.Id,
                                        CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                        IsMenu = false
                                    };

                                    product.AddedToCart += Product_AddedToCart;
                                    productsInCategory.Add(product);
                                }
                            }
                        }

                        // If this is the Menu category (usually ID 6 based on your seed data)
                        if (category.Name == "Menu")
                        {
                            // Load menus as products for display
                            using (var command = new SqlCommand(@"
                        SELECT m.Id, m.Name, m.Price, m.CategoryId, c.Name as CategoryName, 
                               SUM(md.Quantity) as TotalWeight
                        FROM Menus m
                        JOIN Categories c ON m.CategoryId = c.Id
                        JOIN MenuDetails md ON m.Id = md.MenuId
                        WHERE m.CategoryId = @CategoryId
                        GROUP BY m.Id, m.Name, m.Price, m.CategoryId, c.Name", connection))
                            {
                                command.Parameters.AddWithValue("@CategoryId", category.Id);
                                using (var reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        var menuProduct = new ProductItemViewModel
                                        {
                                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                            Name = reader.GetString(reader.GetOrdinal("Name")),
                                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                            // Use the calculated total weight from all menu items
                                            Quantity = reader.GetInt32(reader.GetOrdinal("TotalWeight")),
                                            CategoryId = category.Id,
                                            CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                            IsMenu = true  // Add this flag to indicate it's a menu
                                        };

                                        menuProduct.AddedToCart += Product_AddedToCart;
                                        productsInCategory.Add(menuProduct);
                                    }
                                }
                            }
                        }

                        if (productsInCategory.Any())
                        {
                            CategoriesWithProducts.Add(new CategoryWithProducts
                            {
                                Category = category,
                                Products = new ObservableCollection<ProductItemViewModel>(productsInCategory)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

// TODO: sa fac logica pentru ultimele 6 butoane
// TODO: sa bag butoane pentru alergeni pentru fiecare produs