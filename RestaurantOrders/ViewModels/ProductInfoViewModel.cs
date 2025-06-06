﻿using Microsoft.Data.SqlClient;
using RestaurantOrders.Models;
using RestaurantOrders.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RestaurantOrders.Infrastructure.Config;
using System.Windows.Media;
using RestaurantOrders.Infrastructure.Config.Models;

namespace RestaurantOrders.ViewModels
{
    public class ProductInfoViewModel : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private decimal _price;
        private int _quantity;
        private int _categoryId;
        private int _stock;
        private string _stockText;
        private Brush _stockColor;
        private int _stockThreshold = int.TryParse(AppConfig.QuantityThreshold?.QuantityThreshold, out int threshold) ? threshold : -1;
        private string _categoryName;
        private bool _isMenu;
        private ObservableCollection<MenuItemViewModel> _menuItems = new ObservableCollection<MenuItemViewModel>();
        private ObservableCollection<AllergenInfoViewModel> _allergens = new ObservableCollection<AllergenInfoViewModel>();

        public ProductInfoViewModel(int productId, bool isMenu)
        {
            Id = productId;
            IsMenu = isMenu;
            CommandClose = new RelayCommand(CloseWindow);
            LoadProductInfo();
        }

        #region Properties

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

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Stock
        {
            get => _stock;
            set
            {
                if (_stock != value)
                {
                    _stock = value;
                    UpdateStockStatus();
                    OnPropertyChanged();
                }
            }
        }

        public string StockText
        {
            get => _stockText;
            private set
            {
                if (_stockText != value)
                {
                    _stockText = value;
                    OnPropertyChanged();
                }
            }
        }

        public Brush StockColor
        {
            get => _stockColor;
            private set
            {
                if (_stockColor != value)
                {
                    _stockColor = value;
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

        public bool IsMenu
        {
            get => _isMenu;
            set
            {
                if (_isMenu != value)
                {
                    _isMenu = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<MenuItemViewModel> MenuItems
        {
            get => _menuItems;
            set
            {
                if (_menuItems != value)
                {
                    _menuItems = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<AllergenInfoViewModel> Allergens
        {
            get => _allergens;
            set
            {
                if (_allergens != value)
                {
                    _allergens = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasAllergens => Allergens.Count > 0;
        public bool HasNoAllergens => Allergens.Count == 0;

        #endregion

        #region Commands

        public ICommand CommandClose { get; }

        #endregion

        #region Command Methods

        private void CloseWindow()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Private Methods

        private void UpdateStockStatus()
        {
            if (_stock > _stockThreshold)
            {
                StockText = "In Stock";
                StockColor = Brushes.Green;
            }
            else if (_stock > 0)
            {
                StockText = "Almost out of Stock";
                StockColor = Brushes.Orange;
            }
            else
            {
                StockText = "Not in Stock";
                StockColor = Brushes.Red;
            }
        }


        private void LoadProductInfo()
        {
            try
            {
                if (IsMenu)
                {
                    LoadMenuInfo();
                }
                else
                {
                    LoadProductDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product information: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProductDetails()
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
            {
                connection.Open();

                using (var command = new SqlCommand(@"
                    SELECT p.Id, p.Name, p.Price, p.Quantity, p.CategoryId, c.Name AS CategoryName , rs.StockQuantity AS Stock
                    FROM Products p
                    JOIN Categories c ON p.CategoryId = c.Id
                    JOIN RestaurantStocks rs ON p.Id = rs.ProductId
                    WHERE p.Id = @ProductId", connection))
                {
                    command.Parameters.AddWithValue("@ProductId", Id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            Name = reader.GetString(reader.GetOrdinal("Name"));
                            Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));
                            CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                            CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"));
                            Stock = reader.GetInt32(reader.GetOrdinal("Stock"));
                        }
                    }
                }

                LoadProductAllergens(connection);
            }
        }

        private void LoadMenuInfo()
        {
            using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
            {
                connection.Open();

                using (var command = new SqlCommand(@"
                    SELECT m.Id, m.Name, m.Price, m.CategoryId, c.Name AS CategoryName, 
                           SUM(md.Quantity) AS TotalWeight
                    FROM Menus m
                    JOIN Categories c ON m.CategoryId = c.Id
                    JOIN MenuDetails md ON m.Id = md.MenuId
                    WHERE m.Id = @MenuId
                    GROUP BY m.Id, m.Name, m.Price, m.CategoryId, c.Name", connection))
                {
                    command.Parameters.AddWithValue("@MenuId", Id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id"));
                            Name = reader.GetString(reader.GetOrdinal("Name"));
                            Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                            Quantity = reader.GetInt32(reader.GetOrdinal("TotalWeight"));
                            CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                            CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"));
                        }
                    }
                }

                LoadMenuItems(connection);

                LoadMenuAllergens(connection);
            }
        }

        private void LoadMenuItems(SqlConnection connection)
        {
            MenuItems.Clear();

            using (var command = new SqlCommand(@"
                SELECT p.Id, p.Name, p.Price, md.Quantity
                FROM MenuDetails md
                JOIN Products p ON md.ProductId = p.Id
                WHERE md.MenuId = @MenuId
                ORDER BY p.Name", connection))
            {
                command.Parameters.AddWithValue("@MenuId", Id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MenuItems.Add(new MenuItemViewModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                        });
                    }
                }
            }
        }

        private void LoadProductAllergens(SqlConnection connection)
        {
            Allergens.Clear();

            using (var command = new SqlCommand("GetAllergensByProductId", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductId", Id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Allergens.Add(new AllergenInfoViewModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }
                }
            }

            OnPropertyChanged(nameof(HasAllergens));
            OnPropertyChanged(nameof(HasNoAllergens));
        }

        private void LoadMenuAllergens(SqlConnection connection)
        {
            Allergens.Clear();

            using (var command = new SqlCommand("GetAllergensByMenuId", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@MenuId", Id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Allergens.Add(new AllergenInfoViewModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        });
                    }
                }
            }

            OnPropertyChanged(nameof(HasAllergens));
            OnPropertyChanged(nameof(HasNoAllergens));
        }

        #endregion

        #region Events

        public event EventHandler RequestClose;

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}