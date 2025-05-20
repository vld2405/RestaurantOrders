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
    public class DeleteCategoryViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly RestaurantDbContext _dbContext;

        private bool _isLoading = false;
        private bool _isEmptyState = false;
        private bool _isClosing = false;

        private string _searchTerm = string.Empty;
        private CategoryWithCountViewModel _selectedCategory;

        private ObservableCollection<CategoryWithCountViewModel> _categories = new ObservableCollection<CategoryWithCountViewModel>();
        private ObservableCollection<CategoryWithCountViewModel> _filteredCategories = new ObservableCollection<CategoryWithCountViewModel>();

        public DeleteCategoryViewModel()
        {
            _dbContext = new RestaurantDbContext();

            CommandDeleteCategory = new RelayCommand(DeleteCategory, () => IsCategorySelected);
            CommandCancel = new RelayCommand(Cancel);
            CommandResetFilter = new RelayCommand(ResetFilter);

            LoadCategories();
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
                    FilterCategories();
                }
            }
        }

        public CategoryWithCountViewModel SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsCategorySelected));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsCategorySelected => SelectedCategory != null;

        public ObservableCollection<CategoryWithCountViewModel> Categories
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

        public ObservableCollection<CategoryWithCountViewModel> FilteredCategories
        {
            get => _filteredCategories;
            set
            {
                if (_filteredCategories != value)
                {
                    _filteredCategories = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand CommandDeleteCategory { get; }
        public ICommand CommandCancel { get; }
        public ICommand CommandResetFilter { get; }

        #endregion

        #region Command Methods

        private void DeleteCategory()
        {
            if (SelectedCategory == null)
                return;

            // Check if category has associated products or menus
            if (SelectedCategory.ProductCount > 0 || SelectedCategory.MenuCount > 0)
            {
                string message = "This category cannot be deleted because it is in use:\n";
                if (SelectedCategory.ProductCount > 0)
                    message += $"- Products: {SelectedCategory.ProductCount}\n";
                if (SelectedCategory.MenuCount > 0)
                    message += $"- Menus: {SelectedCategory.MenuCount}\n";
                message += "\nYou must reassign or delete these items first.";

                MessageBox.Show(message, "Cannot Delete Category", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Confirm deletion
            if (MessageBox.Show($"Are you sure you want to delete the category '{SelectedCategory.Name}'?",
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

                    using (var command = new SqlCommand("DeleteCategory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CategoryId", SelectedCategory.Id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int result = reader.GetInt32(reader.GetOrdinal("Result"));
                                string message = reader.GetString(reader.GetOrdinal("Message"));

                                if (result > 0)
                                {
                                    MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                                    // Raise the CategoryDeleted event
                                    CategoryDeleted?.Invoke(this, EventArgs.Empty);

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
        }

        #endregion

        #region Helper Methods

        private void LoadCategories()
        {
            if (_isClosing) return;

            try
            {
                IsLoading = true;
                Categories.Clear();

                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    using (var command = new SqlCommand(@"
                        SELECT c.Id, c.Name, 
                               (SELECT COUNT(*) FROM Products p WHERE p.CategoryId = c.Id) AS ProductCount,
                               (SELECT COUNT(*) FROM Menus m WHERE m.CategoryId = c.Id) AS MenuCount
                        FROM Categories c
                        ORDER BY c.Name", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var category = new CategoryWithCountViewModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    ProductCount = reader.GetInt32(reader.GetOrdinal("ProductCount")),
                                    MenuCount = reader.GetInt32(reader.GetOrdinal("MenuCount"))
                                };

                                Categories.Add(category);
                            }
                        }
                    }
                }

                FilterCategories();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterCategories()
        {
            try
            {
                FilteredCategories.Clear();

                var query = Categories.AsEnumerable();

                // Apply search term filter
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    query = query.Where(c => c.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
                }

                // Add filtered categories to collection
                foreach (var category in query)
                {
                    FilteredCategories.Add(category);
                }

                // Update empty state flag
                IsEmptyState = FilteredCategories.Count == 0;
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Events

        public event EventHandler RequestClose;
        public event EventHandler CategoryDeleted;

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

    // Create a new ViewModel for category with count information
    public class CategoryWithCountViewModel : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private int _productCount;
        private int _menuCount;

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

        public int MenuCount
        {
            get => _menuCount;
            set
            {
                if (_menuCount != value)
                {
                    _menuCount = value;
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