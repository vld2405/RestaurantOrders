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
    public class DeleteMenuViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly RestaurantDbContext _dbContext;

        private bool _isLoading = false;
        private bool _isEmptyState = false;
        private bool _isClosing = false;

        private string _searchTerm = string.Empty;
        private CategoryViewModel _selectedCategory;
        private MenuDetailsViewModel _selectedMenu;

        private ObservableCollection<CategoryViewModel> _categories = new ObservableCollection<CategoryViewModel>();
        private ObservableCollection<MenuDetailsViewModel> _menus = new ObservableCollection<MenuDetailsViewModel>();
        private ObservableCollection<MenuDetailsViewModel> _filteredMenus = new ObservableCollection<MenuDetailsViewModel>();

        public DeleteMenuViewModel()
        {
            _dbContext = new RestaurantDbContext();

            CommandDeleteMenu = new RelayCommand(DeleteMenu, () => IsMenuSelected);
            CommandCancel = new RelayCommand(Cancel);
            CommandResetFilter = new RelayCommand(ResetFilter);

            LoadCategories();
            LoadMenus();
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
                    FilterMenus();
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
                    FilterMenus();
                }
            }
        }

        public MenuDetailsViewModel SelectedMenu
        {
            get => _selectedMenu;
            set
            {
                if (_selectedMenu != value)
                {
                    _selectedMenu = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsMenuSelected));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsMenuSelected => SelectedMenu != null;

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

        public ObservableCollection<MenuDetailsViewModel> Menus
        {
            get => _menus;
            set
            {
                if (_menus != value)
                {
                    _menus = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<MenuDetailsViewModel> FilteredMenus
        {
            get => _filteredMenus;
            set
            {
                if (_filteredMenus != value)
                {
                    _filteredMenus = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand CommandDeleteMenu { get; }
        public ICommand CommandCancel { get; }
        public ICommand CommandResetFilter { get; }

        #endregion

        #region Command Methods

        private void DeleteMenu()
        {
            if (SelectedMenu == null)
                return;

            // Check if menu has active orders
            if (SelectedMenu.ActiveOrderCount > 0)
            {
                MessageBox.Show($"This menu cannot be deleted because it is used in {SelectedMenu.ActiveOrderCount} active orders.\nPlease wait until these orders are completed or canceled.",
                    "Cannot Delete Menu", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Confirm deletion
            if (MessageBox.Show($"Are you sure you want to delete the menu '{SelectedMenu.Name}'?\n\nThis contains {SelectedMenu.ProductCount} products and will permanently remove this menu.",
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

                    using (var command = new SqlCommand("DeleteMenu", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MenuId", SelectedMenu.Id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int result = reader.GetInt32(reader.GetOrdinal("Result"));
                                string message = reader.GetString(reader.GetOrdinal("Message"));

                                if (result > 0)
                                {
                                    MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                                    // Raise the MenuDeleted event
                                    MenuDeleted?.Invoke(this, EventArgs.Empty);

                                    // Close the window
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
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            SelectedCategory = Categories.FirstOrDefault(c => c.Id == -1); // All Categories option
        }

        #endregion

        #region Helper Methods

        private void LoadCategories()
        {
            if (_isClosing) return;

            try
            {
                Categories.Clear();

                // Add "All Categories" option
                Categories.Add(new CategoryViewModel { Id = -1, Name = "All Categories" });

                // Add categories from database
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

                // Select "All Categories" by default
                SelectedCategory = Categories.FirstOrDefault();
            }
            catch (Exception ex)
            {
                if (!_isClosing)
                    MessageBox.Show($"Failed to load categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMenus()
        {
            if (_isClosing) return;

            try
            {
                IsLoading = true;
                Menus.Clear();

                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    // Get all menus with product count and active order count
                    using (var command = new SqlCommand(@"
                        SELECT m.Id, m.Name, m.Price, m.CategoryId, c.Name as CategoryName,
                               (SELECT COUNT(*) FROM MenuDetails md WHERE md.MenuId = m.Id) AS ProductCount,
                               (SELECT COUNT(*) FROM OrderDetails od 
                                INNER JOIN Orders o ON od.OrderId = o.Id
                                WHERE od.MenuId = m.Id AND o.OrderState IN (0, 1, 2)) AS ActiveOrderCount
                        FROM Menus m
                        JOIN Categories c ON m.CategoryId = c.Id
                        ORDER BY m.Name", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var menu = new MenuDetailsViewModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                    CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                                    ProductCount = reader.GetInt32(reader.GetOrdinal("ProductCount")),
                                    ActiveOrderCount = reader.GetInt32(reader.GetOrdinal("ActiveOrderCount"))
                                };

                                Menus.Add(menu);
                            }
                        }
                    }
                }

                FilterMenus();
            }
            catch (Exception ex)
            {
                if (!_isClosing)
                    MessageBox.Show($"Failed to load menus: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterMenus()
        {
            try
            {
                FilteredMenus.Clear();

                var query = Menus.AsEnumerable();

                // Apply search term filter
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    query = query.Where(m => m.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
                }

                // Apply category filter
                if (SelectedCategory != null && SelectedCategory.Id != -1)
                {
                    query = query.Where(m => m.CategoryId == SelectedCategory.Id);
                }

                // Add filtered menus to collection
                foreach (var menu in query)
                {
                    FilteredMenus.Add(menu);
                }

                // Update empty state flag
                IsEmptyState = FilteredMenus.Count == 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering menus: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Events

        public event EventHandler RequestClose;
        public event EventHandler MenuDeleted;

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

    // Create a new ViewModel for menu with details information
    public class MenuDetailsViewModel : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private decimal _price;
        private int _categoryId;
        private string _categoryName;
        private int _productCount;
        private int _activeOrderCount;

        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        public int CategoryId
        {
            get => _categoryId;
            set
            {
                if (_categoryId != value)
                {
                    _categoryId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CategoryName
        {
            get => _categoryName;
            set
            {
                if (_categoryName != value)
                {
                    _categoryName = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ProductCount
        {
            get => _productCount;
            set
            {
                if (_productCount != value)
                {
                    _productCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ActiveOrderCount
        {
            get => _activeOrderCount;
            set
            {
                if (_activeOrderCount != value)
                {
                    _activeOrderCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}