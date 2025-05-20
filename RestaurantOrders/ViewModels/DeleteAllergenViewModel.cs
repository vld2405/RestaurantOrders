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
    public class DeleteAllergenViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly RestaurantDbContext _dbContext;

        private bool _isLoading = false;
        private bool _isEmptyState = false;
        private bool _isClosing = false;

        private string _searchTerm = string.Empty;
        private AllergenInfoViewModel _selectedAllergen;

        private ObservableCollection<AllergenInfoViewModel> _allergens = new ObservableCollection<AllergenInfoViewModel>();
        private ObservableCollection<AllergenInfoViewModel> _filteredAllergens = new ObservableCollection<AllergenInfoViewModel>();

        public DeleteAllergenViewModel()
        {
            _dbContext = new RestaurantDbContext();

            CommandDeleteAllergen = new RelayCommand(DeleteAllergen, () => IsAllergenSelected);
            CommandCancel = new RelayCommand(Cancel);
            CommandResetFilter = new RelayCommand(ResetFilter);

            LoadAllergens();
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
                    FilterAllergens();
                }
            }
        }

        public AllergenInfoViewModel SelectedAllergen
        {
            get => _selectedAllergen;
            set
            {
                if (_selectedAllergen != value)
                {
                    _selectedAllergen = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsAllergenSelected));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsAllergenSelected => SelectedAllergen != null;

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

        public ObservableCollection<AllergenInfoViewModel> FilteredAllergens
        {
            get => _filteredAllergens;
            set
            {
                if (_filteredAllergens != value)
                {
                    _filteredAllergens = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand CommandDeleteAllergen { get; }
        public ICommand CommandCancel { get; }
        public ICommand CommandResetFilter { get; }

        #endregion

        #region Command Methods

        private void DeleteAllergen()
        {
            if (SelectedAllergen == null)
                return;

            if (MessageBox.Show($"Are you sure you want to delete the allergen '{SelectedAllergen.Name}'?",
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

                    using (var command = new SqlCommand("DeleteAllergen", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@AllergenId", SelectedAllergen.Id);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int result = reader.GetInt32(reader.GetOrdinal("Result"));
                                string message = reader.GetString(reader.GetOrdinal("Message"));

                                if (result > 0)
                                {
                                    MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                                    AllergenDeleted?.Invoke(this, EventArgs.Empty);

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

        private void LoadAllergens()
        {
            if (_isClosing) return;

            try
            {
                IsLoading = true;
                Allergens.Clear();

                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    using (var command = new SqlCommand("GetAllAllergens", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var allergen = new AllergenInfoViewModel
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Name = reader.GetString(reader.GetOrdinal("Name"))
                                };

                                Allergens.Add(allergen);
                            }
                        }
                    }
                }

                FilterAllergens();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterAllergens()
        {
            try
            {
                FilteredAllergens.Clear();

                var query = Allergens.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    query = query.Where(a => a.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));
                }

                foreach (var allergen in query)
                {
                    FilteredAllergens.Add(allergen);
                }

                IsEmptyState = FilteredAllergens.Count == 0;
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Events

        public event EventHandler RequestClose;
        public event EventHandler AllergenDeleted;

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