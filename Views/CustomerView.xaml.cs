// File: Laundry/Views/CustomerView.xaml.cs
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Laundry.Data;
using Laundry.Models;
using Laundry.Windows;
using Microsoft.EntityFrameworkCore;

namespace Laundry.Views
{
    public partial class CustomerView : UserControl
    {
        private LaundryDbContext _context;
        
        public CustomerView()
        {
            InitializeComponent();
            
            _context = new LaundryDbContext();
            LoadCustomers();
        }
        
        private void LoadCustomers(string searchText = "")
        {
            try
            {
                var query = _context.Customers.AsQueryable();
                
                // Apply search filter
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(c => 
                        c.Name.Contains(searchText) ||
                        c.Phone.Contains(searchText) ||
                        c.Email.Contains(searchText));
                }
                
                var customers = query
                    .OrderBy(c => c.Name)
                    .ToList();
                
                CustomersDataGrid.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadCustomers(SearchTextBox.Text);
        }
        
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Clear();
            LoadCustomers();
        }
        
        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            var customerWindow = new CustomerWindow();
            customerWindow.Owner = Window.GetWindow(this);
            
            if (customerWindow.ShowDialog() == true)
            {
                LoadCustomers(SearchTextBox.Text);
            }
        }
        
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int customerId = Convert.ToInt32(button.Tag);
            
            var customerWindow = new CustomerWindow(customerId);
            customerWindow.Owner = Window.GetWindow(this);
            
            if (customerWindow.ShowDialog() == true)
            {
                LoadCustomers(SearchTextBox.Text);
            }
        }
        
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int customerId = Convert.ToInt32(button.Tag);
            
            // Check if customer has orders
            var hasOrders = _context.Orders.Any(o => o.CustomerId == customerId);
            if (hasOrders)
            {
                MessageBox.Show("Tidak dapat menghapus pelanggan karena memiliki pesanan terkait.", 
                                "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var result = MessageBox.Show("Anda yakin ingin menghapus pelanggan ini?", 
                                         "Konfirmasi", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var customer = _context.Customers.Find(customerId);
                    if (customer != null)
                    {
                        _context.Customers.Remove(customer);
                        _context.SaveChanges();
                        
                        MessageBox.Show("Pelanggan berhasil dihapus.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadCustomers(SearchTextBox.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}