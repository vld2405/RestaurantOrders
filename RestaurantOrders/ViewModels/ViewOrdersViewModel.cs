using Microsoft.Data.SqlClient;
using RestaurantOrders.Database.Enums;
using RestaurantOrders.Infrastructure.Config;
using RestaurantOrders.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace RestaurantOrders.ViewModels
{
    public class ViewOrdersViewModel : INotifyPropertyChanged
    {
        private bool _isLoading = false;
        private bool _isEmptyState = false;
        private string _searchTerm = string.Empty;
        private DateTime? _fromDate;
        private DateTime? _toDate;
        private ObservableCollection<OrderViewModel> _orders = new ObservableCollection<OrderViewModel>();
        private ObservableCollection<OrderStateViewModel> _orderStates = new ObservableCollection<OrderStateViewModel>();
        private OrderStateViewModel _selectedOrderState;
        private OrderViewModel _selectedOrder;
        private int _totalOrdersCount = 0;

        public ViewOrdersViewModel()
        {
            // Initialize commands
            CommandFilter = new RelayCommand(FilterOrders);
            CommandResetFilter = new RelayCommand(ResetFilter);
            CommandViewDetails = new RelayCommand<OrderViewModel>(ViewOrderDetails);
            CommandUpdateStatus = new RelayCommand<OrderViewModel>(UpdateOrderStatus);
            CommandCancelOrder = new RelayCommand<OrderViewModel>(CancelOrder);
            CommandClose = new RelayCommand(CloseWindow);

            // Initialize order states for filtering
            InitializeOrderStates();

            // Load orders on startup
            LoadOrders();
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
                }
            }
        }

        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                if (_fromDate != value)
                {
                    _fromDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                if (_toDate != value)
                {
                    _toDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<OrderViewModel> Orders
        {
            get => _orders;
            set
            {
                if (_orders != value)
                {
                    _orders = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<OrderStateViewModel> OrderStates
        {
            get => _orderStates;
            set
            {
                if (_orderStates != value)
                {
                    _orderStates = value;
                    OnPropertyChanged();
                }
            }
        }

        public OrderStateViewModel SelectedOrderState
        {
            get => _selectedOrderState;
            set
            {
                if (_selectedOrderState != value)
                {
                    _selectedOrderState = value;
                    OnPropertyChanged();
                }
            }
        }

        public OrderViewModel SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                if (_selectedOrder != value)
                {
                    _selectedOrder = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TotalOrdersCount
        {
            get => _totalOrdersCount;
            set
            {
                if (_totalOrdersCount != value)
                {
                    _totalOrdersCount = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand CommandFilter { get; }
        public ICommand CommandResetFilter { get; }
        public ICommand CommandViewDetails { get; }
        public ICommand CommandUpdateStatus { get; }
        public ICommand CommandCancelOrder { get; }
        public ICommand CommandClose { get; }

        #endregion

        #region Command Methods

        private void FilterOrders()
        {
            LoadOrders();
        }

        private void ResetFilter()
        {
            SearchTerm = string.Empty;
            FromDate = null;
            ToDate = null;
            SelectedOrderState = OrderStates[0]; // All orders option
            LoadOrders();
        }

        private void ViewOrderDetails(OrderViewModel order)
        {
            if (order == null) return;

            MessageBox.Show($"Order Details for Order #{order.OrderId} will be displayed here.\n\n" +
                           $"Customer: {order.CustomerName}\n" +
                           $"Email: {order.Email}\n" +
                           $"Status: {order.OrderState}\n" +
                           $"Total: {order.TotalOrderValue:N2} RON",
                           "Order Details", MessageBoxButton.OK, MessageBoxImage.Information);

            // In a real implementation, you would open another window to show detailed order information
            // OrderDetailsWindow detailsWindow = new OrderDetailsWindow(order.OrderId);
            // detailsWindow.Owner = Application.Current.MainWindow;
            // detailsWindow.ShowDialog();
        }

        private void UpdateOrderStatus(OrderViewModel order)
        {
            if (order == null) return;

            // Get next status 
            OrderState currentState = (OrderState)Enum.Parse(typeof(OrderState), order.OrderState);
            OrderState nextState;

            switch (currentState)
            {
                case OrderState.Registered:
                    nextState = OrderState.InPreparation;
                    break;
                case OrderState.InPreparation:
                    nextState = OrderState.Delivering;
                    break;
                case OrderState.Delivering:
                    nextState = OrderState.Delivered;
                    break;
                default:
                    MessageBox.Show($"Order #{order.OrderId} is already in final state.",
                                   "Status Update", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
            }

            if (MessageBox.Show($"Update order #{order.OrderId} status from {currentState} to {nextState}?",
                               "Confirm Status Update", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                    {
                        connection.Open();
                        using (var command = new SqlCommand("UPDATE Orders SET OrderState = @OrderState WHERE Id = @OrderId", connection))
                        {
                            command.Parameters.AddWithValue("@OrderState", (int)nextState);
                            command.Parameters.AddWithValue("@OrderId", order.OrderId);
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show($"Order #{order.OrderId} status updated to {nextState}.",
                                               "Status Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show($"Failed to update order status. Order #{order.OrderId} not found.",
                                               "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }

                    // Reload orders to refresh the list
                    LoadOrders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating order status: {ex.Message}",
                                   "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelOrder(OrderViewModel order)
        {
            if (order == null) return;

            if (MessageBox.Show($"Are you sure you want to cancel order #{order.OrderId}?",
                               "Confirm Cancellation", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                    {
                        connection.Open();
                        using (var command = new SqlCommand("UPDATE Orders SET OrderState = @OrderState WHERE Id = @OrderId", connection))
                        {
                            command.Parameters.AddWithValue("@OrderState", (int)OrderState.Canceled);
                            command.Parameters.AddWithValue("@OrderId", order.OrderId);
                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show($"Order #{order.OrderId} has been canceled.",
                                               "Order Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                MessageBox.Show($"Failed to cancel order. Order #{order.OrderId} not found.",
                                               "Cancellation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }

                    // Reload orders to refresh the list
                    LoadOrders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error canceling order: {ex.Message}",
                                   "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CloseWindow()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Helper Methods

        private void InitializeOrderStates()
        {
            OrderStates.Clear();

            // Add "All" option
            OrderStates.Add(new OrderStateViewModel { Id = -1, Name = "All" });

            // Add enum values
            foreach (OrderState state in Enum.GetValues(typeof(OrderState)))
            {
                OrderStates.Add(new OrderStateViewModel
                {
                    Id = (int)state,
                    Name = state.ToString()
                });
            }

            // Set default selected state to "All"
            SelectedOrderState = OrderStates[0];
        }

        private void LoadOrders()
        {
            try
            {
                IsLoading = true;
                Orders.Clear();

                using (var connection = new SqlConnection(AppConfig.ConnectionStrings?.RestaurantOrdersDatabase))
                {
                    connection.Open();

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;

                        // Build the query based on filters
                        var queryBuilder = new System.Text.StringBuilder();
                        queryBuilder.AppendLine(@"
                            SELECT 
                                o.Id AS OrderId,
                                o.UserId,
                                u.FirstName,
                                u.LastName,
                                u.Email,
                                u.PhoneNo,
                                u.Address,
                                o.OrderState,
                                o.CreatedAt,
                                o.EstimatedDeliveryTime,
                                (
                                    SELECT SUM(
                                        CASE 
                                            WHEN od.ProductId IS NOT NULL THEN p.Price * od.Quantity
                                            WHEN od.MenuId IS NOT NULL THEN m.Price * od.Quantity
                                            ELSE 0
                                        END
                                    )
                                    FROM OrderDetails od
                                    LEFT JOIN Products p ON od.ProductId = p.Id
                                    LEFT JOIN Menus m ON od.MenuId = m.Id
                                    WHERE od.OrderId = o.Id
                                ) AS TotalOrderValue
                            FROM 
                                Orders o
                            INNER JOIN 
                                Users u ON o.UserId = u.Id
                            WHERE 1=1");

                        // Add filters
                        if (!string.IsNullOrWhiteSpace(SearchTerm))
                        {
                            queryBuilder.AppendLine("AND (o.Id LIKE @SearchTerm OR u.FirstName LIKE @SearchTerm OR u.LastName LIKE @SearchTerm OR u.Email LIKE @SearchTerm)");
                            command.Parameters.AddWithValue("@SearchTerm", $"%{SearchTerm}%");
                        }

                        if (SelectedOrderState != null && SelectedOrderState.Id >= 0)
                        {
                            queryBuilder.AppendLine("AND o.OrderState = @OrderState");
                            command.Parameters.AddWithValue("@OrderState", SelectedOrderState.Id);
                        }

                        if (FromDate.HasValue)
                        {
                            queryBuilder.AppendLine("AND o.CreatedAt >= @FromDate");
                            command.Parameters.AddWithValue("@FromDate", FromDate.Value.Date);
                        }

                        if (ToDate.HasValue)
                        {
                            queryBuilder.AppendLine("AND o.CreatedAt <= @ToDate");
                            command.Parameters.AddWithValue("@ToDate", ToDate.Value.Date.AddDays(1).AddSeconds(-1));
                        }

                        queryBuilder.AppendLine("ORDER BY o.CreatedAt DESC");

                        command.CommandText = queryBuilder.ToString();

                        try
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var orderState = (OrderState)reader.GetInt32(reader.GetOrdinal("OrderState"));
                                    var firstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? string.Empty : reader.GetString(reader.GetOrdinal("FirstName"));
                                    var lastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? string.Empty : reader.GetString(reader.GetOrdinal("LastName"));

                                    Orders.Add(new OrderViewModel
                                    {
                                        OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                        OrderState = orderState.ToString(),
                                        StatusColor = GetStatusColor(orderState),
                                        CustomerName = $"{firstName} {lastName}".Trim(),
                                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                                        EstimatedDeliveryTime = reader.GetDateTime(reader.GetOrdinal("EstimatedDeliveryTime")),
                                        TotalOrderValue = reader.IsDBNull(reader.GetOrdinal("TotalOrderValue")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TotalOrderValue")),
                                        CanCancel = orderState != OrderState.Delivered && orderState != OrderState.Canceled
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }

                // Check if orders list is empty
                IsEmptyState = Orders.Count == 0;
                TotalOrdersCount = Orders.Count;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        

        private Brush GetStatusColor(OrderState state)
        {
            // Return different colors based on order status
            switch (state)
            {
                case OrderState.Registered:
                    return new SolidColorBrush(Color.FromRgb(25, 118, 210)); // Blue
                case OrderState.InPreparation:
                    return new SolidColorBrush(Color.FromRgb(255, 152, 0));  // Orange
                case OrderState.Delivering:
                    return new SolidColorBrush(Color.FromRgb(156, 39, 176)); // Purple
                case OrderState.Delivered:
                    return new SolidColorBrush(Color.FromRgb(76, 175, 80));  // Green
                case OrderState.Canceled:
                    return new SolidColorBrush(Color.FromRgb(244, 67, 54));  // Red
                default:
                    return new SolidColorBrush(Color.FromRgb(97, 97, 97));   // Gray
            }
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