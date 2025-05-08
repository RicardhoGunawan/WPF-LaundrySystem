using System;
using System.Linq;
using System.Windows;
using Laundry.Data;
using Laundry.Models;
using Microsoft.EntityFrameworkCore;

namespace Laundry.Windows
{
    public partial class PaymentWindow : Window
    {
        private LaundryDbContext _context;
        private int _orderId;
        private Order _order;
        
        public PaymentWindow(int orderId)
        {
            InitializeComponent();
            _context = new LaundryDbContext();
            _orderId = orderId;
            
            LoadOrder();
            LoadPaymentMethods();
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
            OrderDateTextBlock.Text = _order.OrderDate.ToString("dd/MM/yyyy HH:mm");
            CustomerNameTextBlock.Text = _order.Customer.Name;
            TotalAmountTextBlock.Text = _order.FinalAmount.ToString("N0");
            
            // Set default payment amount
            AmountTextBox.Text = _order.FinalAmount.ToString("N0");
        }
        
        private void LoadPaymentMethods()
        {
            var paymentMethods = Enum.GetValues(typeof(PaymentMethod)).Cast<PaymentMethod>();
            PaymentMethodComboBox.ItemsSource = paymentMethods;
            PaymentMethodComboBox.SelectedIndex = 0;
        }
        
        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (PaymentMethodComboBox.SelectedItem == null)
            {
                MessageBox.Show("Pilih metode pembayaran.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(AmountTextBox.Text) || !decimal.TryParse(AmountTextBox.Text.Replace(",", ""), out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Masukkan jumlah pembayaran yang valid.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                var payment = new Payment
                {
                    OrderId = _orderId,
                    PaymentDate = DateTime.Now,
                    Amount = amount,
                    Method = (PaymentMethod)PaymentMethodComboBox.SelectedItem,
                    ReferenceNumber = ReferenceTextBox.Text,
                    Notes = NotesTextBox.Text
                };
                
                _context.Payments.Add(payment);
                
                // Update order status
                _order.IsPaid = true;
                _order.Status = OrderStatus.Processing;
                
                _context.SaveChanges();
                
                MessageBox.Show("Pembayaran berhasil diproses.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
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