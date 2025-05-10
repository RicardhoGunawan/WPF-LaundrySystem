// File: Laundry/Windows/StatusUpdateWindow.xaml.cs
using System;
using System.Linq;
using System.Windows;
using Laundry.Data;
using Laundry.Models;
using Microsoft.EntityFrameworkCore;

namespace Laundry.Windows
{
    public partial class StatusUpdateWindow : Window
    {
        private LaundryDbContext _context;
        private int _orderId;
        private Order _order;
        
        public StatusUpdateWindow(int orderId)
        {
            InitializeComponent();
            _context = new LaundryDbContext();
            _orderId = orderId;
            
            LoadOrder();
            LoadStatuses();
        }
        
        private void LoadOrder()
        {
            _order = _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefault(o => o.OrderId == _orderId);
                
            if (_order == null)
            {
                MessageBox.Show("Pesanan tidak ditemukan.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }
            
            OrderIdTextBlock.Text = _order.OrderId.ToString();
            CustomerNameTextBlock.Text = _order.Customer.Name;
            CurrentStatusTextBlock.Text = _order.Status.ToString();
        }
        
        private void LoadStatuses()
        {
            var statuses = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>();
            StatusComboBox.ItemsSource = statuses;
            
            // Set current status as default
            StatusComboBox.SelectedItem = _order.Status;
        }
        
        // Perbaikan untuk StatusUpdateWindow.xaml.cs
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Pilih status baru.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
    
            var newStatus = (OrderStatus)StatusComboBox.SelectedItem;
    
            try
            {
                // Update order status
                _order.Status = newStatus;
        
                // If status is Completed, set delivery date
                if (newStatus == OrderStatus.Completed && !_order.DeliveryDate.HasValue)
                {
                    _order.DeliveryDate = DateTime.Now;
                }
        
                // Add notes if provided
                if (!string.IsNullOrWhiteSpace(NotesTextBox.Text))
                {
                    _order.Notes += $"\n[{DateTime.Now:dd/MM/yyyy HH:mm}] Status update to {newStatus}: {NotesTextBox.Text}";
                }
        
                _context.SaveChanges();
        
                // Pastikan perubahan disimpan dengan benar
                _context.Entry(_order).Reload(); // Reload entity dari database
        
                MessageBox.Show("Status pesanan berhasil diupdate.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}