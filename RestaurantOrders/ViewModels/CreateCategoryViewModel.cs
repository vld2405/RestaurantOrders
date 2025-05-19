using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Query;
using RestaurantOrders.Infrastructure.Config;
using RestaurantOrders.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RestaurantOrders.ViewModels
{
    public class CreateCategoryViewModel : INotifyPropertyChanged
    {
        public CreateCategoryViewModel()
        {
            CommandSubmit = new RelayCommand(SubmitButton);
            CommandCancel = new RelayCommand(CancelButton);
        }

        private string _categoryName = "";
        private bool _canSubmit = false;

        #region getters-setters
        public bool CanSubmit
        {
            get { return _canSubmit; }

            set
            {
                _canSubmit = value;
                OnPropertyChanged(nameof(CanSubmit));
            }
        }

        public string CategoryName
        {
            get { return _categoryName; }

            set
            {
                _categoryName = value;

                if (_categoryName != "")
                    CanSubmit = true;
                else
                    CanSubmit = false;

                OnPropertyChanged(nameof(CategoryName));
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
            try
            {
                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();
                    using (var command = new SqlCommand("AddCategory", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Name", CategoryName);

                        var result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            int categoryId = Convert.ToInt32(result);

                            if (categoryId == -1)
                            {
                                MessageBox.Show("This category already exists.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            MessageBox.Show("Category added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            OnRequestClose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        #region Property Changed

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
