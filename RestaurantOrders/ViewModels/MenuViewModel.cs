﻿using Microsoft.Data.SqlClient;
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
            CommandShowOrders = new RelayCommand(ShowOrders);
            CommandViewMyOrders = new RelayCommand(ViewOrders);


            CategoriesWithProducts = new ObservableCollection<CategoryWithProducts>();
            FilteredCategoriesWithProducts = CategoriesWithProducts;
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
        private bool _showDiscount = false;
        private int _discountPercentage;

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
        public bool ShowDiscount
        {
            get => _showDiscount;
            set
            {
                _showDiscount = value;
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
        public ICommand CommandShowOrders { get; set; }
        public ICommand CommandViewMyOrders { get; set; }

        #endregion

        #region Command-Methods

        private void AddProduct()
        {
            CreateProductWindow createProductWindow = new CreateProductWindow();
            createProductWindow.Owner = Application.Current.MainWindow;

            createProductWindow.ProductAdded += CreateProductWindow_ProductAdded;

            createProductWindow.ShowDialog();
        }

        private void CreateProductWindow_ProductAdded(object sender, EventArgs e)
        {
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
            DeleteProductWindow deleteProductWindow = new DeleteProductWindow();
            deleteProductWindow.Owner = Application.Current.MainWindow;

            deleteProductWindow.ProductDeleted += DeleteProductWindow_ProductDeleted;

            deleteProductWindow.ShowDialog();
        }

        private void DeleteProductWindow_ProductDeleted(object sender, EventArgs e)
        {
            RefreshCategoriesAndProducts();
        }
        private void DeleteAllergen()
        {
            DeleteAllergenWindow deleteAllergenWindow = new DeleteAllergenWindow();
            deleteAllergenWindow.Owner = Application.Current.MainWindow;

            deleteAllergenWindow.AllergenDeleted += DeleteAllergenWindow_AllergenDeleted;

            deleteAllergenWindow.ShowDialog();
        }

        private void DeleteAllergenWindow_AllergenDeleted(object sender, EventArgs e)
        {
            RefreshCategoriesAndProducts();
        }
        private void DeleteCategory()
        {
            DeleteCategoryWindow deleteCategoryWindow = new DeleteCategoryWindow();
            deleteCategoryWindow.Owner = Application.Current.MainWindow;

            deleteCategoryWindow.CategoryDeleted += DeleteCategoryWindow_CategoryDeleted;

            deleteCategoryWindow.ShowDialog();
        }

        private void DeleteCategoryWindow_CategoryDeleted(object sender, EventArgs e)
        {
            RefreshCategoriesAndProducts();
        }
        private void DeleteMenu()
        {
            DeleteMenuWindow deleteMenuWindow = new DeleteMenuWindow();
            deleteMenuWindow.Owner = Application.Current.MainWindow;

            deleteMenuWindow.MenuDeleted += DeleteMenuWindow_MenuDeleted;

            deleteMenuWindow.ShowDialog();
        }

        private void DeleteMenuWindow_MenuDeleted(object sender, EventArgs e)
        {
            RefreshCategoriesAndProducts();
        }
        private void ViewOrders()
        {
            ViewMyOrdersWindow viewMyOrdersWindow = new ViewMyOrdersWindow(CurrentUser);
            viewMyOrdersWindow.Owner = Application.Current.MainWindow;
            viewMyOrdersWindow.ShowDialog();
        }
        private void ShowOrders()
        {
            ViewOrdersWindow viewOrdersWindow = new ViewOrdersWindow();
            viewOrdersWindow.Owner = Application.Current.MainWindow;
            viewOrdersWindow.ShowDialog();
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

        private void Product_AddedToCart(object sender, EventArgs e)
        {
            UpdateCart();
        }

        private void UpdateCart()
        {
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

            if (AppConfig.OrderDiscount?.OrderDiscount != null)
            {
                if (int.TryParse(AppConfig.OrderDiscount.OrderDiscount, out int parsedDiscount))
                {
                    _discountPercentage = parsedDiscount;
                }
            }
            Console.WriteLine(_discountPercentage);
            decimal total = CartItems.Sum(p => p.Price * p.OrderQuantity);

            ShowDiscount = total > 100;
            CartTotal = ShowDiscount 
                ? Math.Round(total * (1 - (_discountPercentage / 100.0m)), 2)
                : total;

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
                if (!CartItems.Any())
                {
                    MessageBox.Show("Your cart is empty. Please add items to your order.", "Empty Cart", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                bool insufficientStock = false;
                string outOfStockItems = "";
                bool adjustedQuantities = false;

                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    var itemsToRemove = new List<ProductItemViewModel>();

                    foreach (var item in CartItems)
                    {
                        if (item.IsMenu)
                        {
                            using (var command = new SqlCommand(
                                @"SELECT p.Id, p.Name, md.Quantity * @OrderQuantity AS RequiredQuantity, rs.StockQuantity 
                                  FROM MenuDetails md
                                  JOIN Products p ON md.ProductId = p.Id
                                  LEFT JOIN RestaurantStocks rs ON p.Id = rs.ProductId
                                  WHERE md.MenuId = @MenuId", connection))
                            {
                                command.Parameters.AddWithValue("@MenuId", item.Id);
                                command.Parameters.AddWithValue("@OrderQuantity", item.OrderQuantity);

                                using (var reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int requiredQuantity = reader.GetInt32(reader.GetOrdinal("RequiredQuantity"));
                                        int stockQuantity = reader.IsDBNull(reader.GetOrdinal("StockQuantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("StockQuantity"));
                                        string productName = reader.GetString(reader.GetOrdinal("Name"));

                                        if (stockQuantity < requiredQuantity)
                                        {
                                            if (stockQuantity <= 0)
                                            {
                                                insufficientStock = true;
                                                outOfStockItems += $"- {productName} (in menu {item.Name})\n";
                                            }
                                            else
                                            {
                                                adjustedQuantities = true;
                                                int maxPossibleItems = stockQuantity / (requiredQuantity / item.OrderQuantity);
                                                item.OrderQuantity = maxPossibleItems;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (var command = new SqlCommand(
                                @"SELECT StockQuantity FROM RestaurantStocks WHERE ProductId = @ProductId", connection))
                            {
                                command.Parameters.AddWithValue("@ProductId", item.Id);
                                object result = command.ExecuteScalar();
                                int stockQuantity = result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);

                                if (stockQuantity < item.OrderQuantity)
                                {
                                    if (stockQuantity <= 0)
                                    {
                                        insufficientStock = true;
                                        outOfStockItems += $"- {item.Name}\n";
                                        itemsToRemove.Add(item);
                                    }
                                    else
                                    {
                                        adjustedQuantities = true;
                                        item.OrderQuantity = stockQuantity;
                                    }
                                }
                            }
                        }
                    }

                    foreach (var item in itemsToRemove)
                    {
                        CartItems.Remove(item);
                    }

                }

                if (insufficientStock)
                {
                    MessageBox.Show($"The following items are out of stock:\n{outOfStockItems}\nThey have been removed from your cart.",
                        "Out of Stock", MessageBoxButton.OK, MessageBoxImage.Warning);
                    UpdateCart();
                    return;
                }

                if (adjustedQuantities)
                {
                    MessageBox.Show("Some item quantities were adjusted due to limited stock.\nPlease review your cart before placing the order.",
                        "Stock Adjustment", MessageBoxButton.OK, MessageBoxImage.Warning);
                    UpdateCart();
                    return;
                }


                List<int> productIds = new List<int>();
                List<int> menuIds = new List<int>();
                List<int> quantities = new List<int>();

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

                string productIdsStr = string.Join(",", productIds);
                string menuIdsStr = string.Join(",", menuIds);
                string quantitiesStr = string.Join(",", quantities);

                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    using (var command = new SqlCommand("AddOrder", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        int userId = 1;

                        if (CurrentUser != null && CurrentUser.Id > 0)
                        {
                            userId = CurrentUser.Id;
                        }

                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@OrderState", (int)OrderState.Registered);
                        command.Parameters.AddWithValue("@ProductIds", productIdsStr);

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
                                    DateTime estimatedDelivery = reader.GetDateTime(reader.GetOrdinal("EstimatedDeliveryTime"));
                                    string deliveryTimeStr = estimatedDelivery.ToString("hh:mm tt");

                                    UpdateStockQuantities(productIds, menuIds, quantities);

                                    MessageBox.Show($"Your order #{orderId} has been placed successfully!\n\nEstimated delivery time: {deliveryTimeStr}",
                                        "Order Placed", MessageBoxButton.OK, MessageBoxImage.Information);

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
                MessageBox.Show($"An error occurred while placing your order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateStockQuantities(List<int> productIds, List<int> menuIds, List<int> quantities)
        {
            try
            {
                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            for (int i = 0; i < productIds.Count; i++)
                            {
                                using (var command = new SqlCommand(
                                    @"UPDATE RestaurantStocks 
                                      SET StockQuantity = StockQuantity - @Quantity 
                                      WHERE ProductId = @ProductId", connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@ProductId", productIds[i]);
                                    command.Parameters.AddWithValue("@Quantity", quantities[i]);
                                    command.ExecuteNonQuery();
                                }
                            }

                            for (int i = 0; i < menuIds.Count; i++)
                            {
                                using (var command = new SqlCommand(
                                    @"UPDATE rs
                                      SET rs.StockQuantity = rs.StockQuantity - (md.Quantity * @OrderQuantity)
                                      FROM RestaurantStocks rs
                                      JOIN MenuDetails md ON rs.ProductId = md.ProductId
                                      WHERE md.MenuId = @MenuId", connection, transaction))
                                {
                                    command.Parameters.AddWithValue("@MenuId", menuIds[i]);
                                    command.Parameters.AddWithValue("@OrderQuantity", quantities[i]);
                                    command.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Failed to update stock: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating stock: {ex.Message}", "Stock Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Search Functionality

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

        public void RefreshCategoriesAndProducts()
        {
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

            CategoriesWithProducts.Clear();
            CartItems.Clear();
            CartTotal = 0;

            LoadCategoriesAndProducts();

            FilterProducts();
        }

        private void LoadCategoriesAndProducts()
        {
            try
            {
                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

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

                    foreach (var category in categories)
                    {
                        var productsInCategory = new List<ProductItemViewModel>();

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

                        if (category.Name == "Menu")
                        {
                            using (var command = new SqlCommand(
                              @"SELECT m.Id, m.Name, m.Price, m.CategoryId, c.Name as CategoryName, 
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
                                            Quantity = reader.GetInt32(reader.GetOrdinal("TotalWeight")),
                                            CategoryId = category.Id,
                                            CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                            IsMenu = true
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
