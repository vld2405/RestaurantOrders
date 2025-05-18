using RestaurantOrders.Database.Entities;
using RestaurantOrders.Database.Enums;
using RestaurantOrders.Models;
using RestaurantOrders.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            CommandCreateMenu = new RelayCommand(CreateMenu);
            CommandDeleteProduct = new RelayCommand(DeleteProduct);
            CommandDeleteAllergen = new RelayCommand(DeleteAllergen);
            CommandAddCategory = new RelayCommand(AddCategory);
            CommandDeleteCategory = new RelayCommand(DeleteCategory);
        }
        public MenuViewModel(UserType userType) : this() 
        {
            if(userType == UserType.NoAccount)
            {
                IsSubmitEnabled = false;
                IsAdminVisibility = false;
            }
        }
        public MenuViewModel(User user, UserType userType) : this(userType)
        {}

        private bool _isSubmitEnabled = true;
        private bool _isAdminVisibility = true;

        #region getters-setters
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

        }
        private void DeleteCategory()
        {

        }

        // TODO: adminul poate vedea toate comenzile sortate descrescator dupa data si ora
        // TODO: adminul poate schimba starea unei comenzi
        // TODO: adminul poate vedea toate preparatele care se apropie de epuizare

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
