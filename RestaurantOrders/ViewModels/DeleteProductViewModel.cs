using Microsoft.Data.SqlClient;
using RestaurantOrders.Database.Context;
using RestaurantOrders.Infrastructure.Config;
using RestaurantOrders.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace RestaurantOrders.ViewModels
{
    public class DeleteProductViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly RestaurantDbContext _dbContext;

        private bool _isLoading = false;
        private bool _isEmptyState = false;
        private bool _isClosing = false;

        private string _searchTerm = string.Empty;
        private CategoryViewModel _selectedCategory;
        private ProductViewModel _selectedProduct;

        private ObservableCollection<CategoryViewModel> _categories = new ObservableCollection<CategoryViewModel>();
        private ObservableCollection<ProductViewModel> _products = new ObservableCollection<ProductViewModel>();
        private ObservableCollection<ProductViewModel> _filteredProducts = new ObservableCollection<ProductViewModel>();

        public DeleteProductViewModel()
        {
            _dbContext = new RestaurantDbContext();

            CommandDeleteProduct = new RelayCommand(DeleteProduct, () => IsProductSelected);
            CommandCancel = new RelayCommand(Cancel);
            CommandResetFilter = new RelayCommand(ResetFilter);

            LoadCategories();
            LoadProducts();
        }

        #region Properties

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsEmptyState
        {
            get => _isEmptyState;
            set
            {
                if (_isEmptyState != value)
                {
                    _isEmptyState = value;
                    OnPropertyChanged();
                }
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

        public CategoryViewModel SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
                    FilterProducts();
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
                    OnPropertyChanged(nameof(IsProductSelected));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsProductSelected => SelectedProduct != null;

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

        public ObservableCollection<ProductViewModel> Products
        {
            get => _products;
            set
            {
                if (_products != value)
                {
                    _products = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ProductViewModel> FilteredProducts
        {
            get => _filteredProducts;
            set
            {
                if (_filteredProducts != value)
                {
                    _filteredProducts = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand CommandDeleteProduct { get; }
        public ICommand CommandCancel { get; }
        public ICommand CommandResetFilter { get; }

        #endregion

        #region Command Methods

        private void DeleteProduct()
        {
            if (SelectedProduct == null)
                return;

            if (MessageBox.Show($"Are you sure you want to delete the product '{SelectedProduct.Name}'?",
                               "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                IsLoading = true;

                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    using (var command = new SqlCommand("DeleteProduct", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ProductId", SelectedProduct.Id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int result = reader.GetInt32(reader.GetOrdinal("Result"));
                                string message = reader.GetString(reader.GetOrdinal("Message"));

                                if (result > 0)
                                {
                                    MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                                    ProductDeleted?.Invoke(this, EventArgs.Empty);

                                    OnRequestClose();
                                }
                                else
                                {
                                    MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void Cancel()
        {
            OnRequestClose();
        }

        private void ResetFilter()
        {
            SearchTerm = string.Empty;
            SelectedCategory = null;
        }

        #endregion

        #region Helper Methods

        private void LoadCategories()
        {
            if (_isClosing) return;

            try
            {
                Categories.Clear();

                Categories.Add(new CategoryViewModel { Id = -1, Name = "All Categories" });

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

                SelectedCategory = Categories.FirstOrDefault();
            }
            catch (Exception ex)
            {
            }
        }

        private void LoadProducts()
        {
            if (_isClosing) return;

            try
            {
                IsLoading = true;
                Products.Clear();

                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    using (var command = new SqlCommand(@"
                        SELECT p.Id, p.Name, p.Price, p.Quantity, p.CategoryId, c.Name as CategoryName 
                        FROM Products p
                        JOIN Categories c ON p.CategoryId = c.Id
                        WHERE c.Name <> 'Menu'
                        ORDER BY p.Name", connection))
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

                                Products.Add(product);
                            }
                        }
                    }
                }

                FilterProducts();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterProducts()
        {
            try
            {
                FilteredProducts.Clear();

                var query = Products.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    query = query.Where(p => p.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
                }

                if (SelectedCategory != null && SelectedCategory.Id != -1)
                {
                    query = query.Where(p => p.CategoryId == SelectedCategory.Id);
                }

                foreach (var product in query)
                {
                    FilteredProducts.Add(product);
                }

                IsEmptyState = FilteredProducts.Count == 0;
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Events

        public event EventHandler RequestClose;
        public event EventHandler ProductDeleted;

        protected virtual void OnRequestClose()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _isClosing = true;
            _dbContext?.Dispose();
        }

        #endregion
    }
}