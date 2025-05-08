using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Laundry.Data;
using Laundry.Models;
using Laundry.Windows;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Laundry.Views
{
    public class BoolToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPaid)
            {
                return isPaid ? "Sudah Dibayar" : "Belum Dibayar";
            }
            return "Belum Dibayar";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                bool reverse = parameter != null && parameter.ToString() == "reverse";
                
                if (reverse)
                    return boolValue ? Visibility.Collapsed : Visibility.Visible;
                else
                    return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class OrderListView : UserControl
    {
        private LaundryDbContext _context;
        
        public OrderListView()
        {
            InitializeComponent();
            
            // Add Resources
            Resources.Add("BoolToStatusConverter", new BoolToStatusConverter());
            Resources.Add("BoolToVisibilityConverter", new BoolToVisibilityConverter());
            
            _context = new LaundryDbContext();
            
            // Initialize status filter
            var statusItems = Enum.GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(s => s.ToString())
                .ToList();
            
            statusItems.Insert(0, "Semua Status");
            StatusComboBox.ItemsSource = statusItems;
            StatusComboBox.SelectedIndex = 0;
            
            LoadOrders();
        }
        
        private void LoadOrders(string searchText = "", string statusFilter = "")
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.Customer)
                    .AsQueryable();
                
                // Apply search filter
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(o => 
                        o.OrderId.ToString().Contains(searchText) ||
                        o.Customer.Name.Contains(searchText) ||
                        o.Customer.Phone.Contains(searchText));
                }
                
                // Apply status filter
                if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "Semua Status")
                {
                    if (Enum.TryParse<OrderStatus>(statusFilter, out var status))
                    {
                        query = query.Where(o => o.Status == status);
                    }
                }
                
                var orders = query
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();
                
                OrdersDataGrid.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text;
            string statusFilter = StatusComboBox.SelectedItem?.ToString() ?? "";
            
            LoadOrders(searchText, statusFilter);
        }
        
        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text;
            string statusFilter = StatusComboBox.SelectedItem?.ToString() ?? "";
            
            LoadOrders(searchText, statusFilter);
        }
        
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Clear();
            StatusComboBox.SelectedIndex = 0;
            LoadOrders();
        }
        
        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int orderId = Convert.ToInt32(button.Tag);
            
            var orderDetailWindow = new OrderDetailWindow(orderId);
            orderDetailWindow.Owner = Window.GetWindow(this);
            orderDetailWindow.ShowDialog();
            
            LoadOrders(SearchTextBox.Text, StatusComboBox.SelectedItem?.ToString() ?? "");
        }
        
        private void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int orderId = Convert.ToInt32(button.Tag);
            
            var paymentWindow = new PaymentWindow(orderId);
            paymentWindow.Owner = Window.GetWindow(this);
            
            if (paymentWindow.ShowDialog() == true)
            {
                LoadOrders(SearchTextBox.Text, StatusComboBox.SelectedItem?.ToString() ?? "");
            }
        }
        
        private void UpdateStatusButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int orderId = Convert.ToInt32(button.Tag);
            
            var statusUpdateWindow = new StatusUpdateWindow(orderId);
            statusUpdateWindow.Owner = Window.GetWindow(this);
            
            if (statusUpdateWindow.ShowDialog() == true)
            {
                LoadOrders(SearchTextBox.Text, StatusComboBox.SelectedItem?.ToString() ?? "");
            }
        }
    }
}