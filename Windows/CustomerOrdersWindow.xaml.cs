using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Laundry.Data;
using Laundry.Models;
using Microsoft.EntityFrameworkCore;

namespace Laundry.Windows
{
    public partial class CustomerOrdersWindow : Window
    {
        private LaundryDbContext _context;
        private int _customerId;
        private Customer _customer;
        
        public CustomerOrdersWindow(int customerId)
        {
            InitializeComponent();
            _context = new LaundryDbContext();
            _customerId = customerId;
            
            LoadCustomer();
            LoadOrders();
        }
        
        private void LoadCustomer()
        {
            _customer = _context.Customers.Find(_customerId);
            if (_customer == null)
            {
                MessageBox.Show("Pelanggan tidak ditemukan.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            
            NameTextBlock.Text = _customer.Name;
            PhoneTextBlock.Text = _customer.Phone;
            AddressTextBlock.Text = _customer.Address;
            RegisterDateTextBlock.Text = _customer.RegisterDate.ToString("dd/MM/yyyy");
            
            Title = $"Riwayat Pesanan - {_customer.Name}";
        }
        
        private void LoadOrders()
        {
            var orders = _context.Orders
                .Where(o => o.CustomerId == _customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
                
            OrdersDataGrid.ItemsSource = orders;
        }
        
        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int orderId = Convert.ToInt32(button.Tag);
            
            var orderDetailWindow = new OrderDetailWindow(orderId);
            orderDetailWindow.Owner = this;
            orderDetailWindow.ShowDialog();
            
            LoadOrders();
        }
        
        private void NewOrderButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
            
            MessageBox.Show($"Silakan buat pesanan baru untuk {_customer.Name} di tab 'Pesanan Baru'.", 
                            "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}