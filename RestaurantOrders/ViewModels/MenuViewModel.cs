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
            CommandDeleteCategory = new RelayCommand(DeleteCategory);
            CommandPlaceOrder = new RelayCommand(PlaceOrder, CanPlaceOrder);

            CategoriesWithProducts = new ObservableCollection<CategoryWithProducts>();
            CartItems = new ObservableCollection<ProductItemViewModel>();

            LoadCategoriesAndProducts();
        }
        public MenuViewModel(UserType userType) : this()
        {
            if (userType == UserType.NoAccount)
            {
                IsSubmitEnabled = false;
                IsAdminVisibility = false;
            }
            if (userType == UserType.Client)
            {
                IsAdminVisibility = false;
            }
        }

        public MenuViewModel(User user, UserType userType) : this(userType)
        { }

        private bool _isSubmitEnabled = true;
        private bool _isAdminVisibility = true;

        private ObservableCollection<CategoryWithProducts> _categoriesWithProducts;
        private decimal _cartTotal;
        private ObservableCollection<ProductItemViewModel> _cartItems;

        #region getters-setters

        public ObservableCollection<CategoryWithProducts> CategoriesWithProducts
        {
            get => _categoriesWithProducts;
            set
            {
                _categoriesWithProducts = value;
                OnPropertyChanged();
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
        public ICommand CommandAddCategory { get; set; }
        public ICommand CommandDeleteCategory { get; set; }
        public ICommand CommandPlaceOrder { get; set; }

        #endregion

        #region Command-Methods

        private void AddProduct()
        {
            CreateProductWindow createProductWindow = new CreateProductWindow();
            createProductWindow.Owner = Application.Current.MainWindow;
            createProductWindow.ShowDialog();
        }
        private void AddAllergen()
        {
            CreateAllergenWindow createAllergenWindow = new CreateAllergenWindow();
            createAllergenWindow.Owner = Application.Current.MainWindow;
            createAllergenWindow.ShowDialog();
        }
        private void CreateMenu()
        {

        }
        private void DeleteProduct()
        {

        }
        private void DeleteAllergen()
        {

        }
        private void AddCategory()
        {
            CreateCategoryWindow createCategoryWindow = new CreateCategoryWindow();
            createCategoryWindow.Owner = Application.Current.MainWindow;
            createCategoryWindow.ShowDialog();
        }
        private void DeleteCategory()
        {

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
            // Implement order placement logic
            MessageBox.Show("Your order has been placed!", "Order Placed", MessageBoxButton.OK, MessageBoxImage.Information);

            // Reset cart
            foreach (var product in CartItems)
            {
                product.OrderQuantity = 0;
                product.TempQuantity = 0;
            }

            UpdateCart();
        }

        #endregion

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

                    // Load products for each category
                    foreach (var category in categories)
                    {
                        var productsInCategory = new List<ProductItemViewModel>();
                        using (var command = new SqlCommand("SELECT Id, Name, Quantity, Price FROM Products WHERE CategoryId = @CategoryId", connection))
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
                                        CategoryId = category.Id
                                    };

                                    // Subscribe to the AddedToCart event instead of PropertyChanged
                                    product.AddedToCart += Product_AddedToCart;

                                    productsInCategory.Add(product);
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
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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