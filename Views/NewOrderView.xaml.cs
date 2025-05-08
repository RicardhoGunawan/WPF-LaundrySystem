using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Laundry.Data;
using Laundry.Models;
using Laundry.Windows;
using Microsoft.EntityFrameworkCore;

namespace Laundry
{
    public partial class NewOrderView : UserControl
    {
        private LaundryDbContext _context;
        private ObservableCollection<OrderItemViewModel> _orderItems;
        private decimal _totalAmount = 0;
        private decimal _discountAmount = 0;
        private decimal _finalAmount = 0;
        
        public class OrderItemViewModel
        {
            public int No { get; set; }
            public int ServiceId { get; set; }
            public string ServiceName { get; set; }
            public decimal Weight { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Subtotal { get; set; }
        }
        
        public NewOrderView()
        {
            InitializeComponent();
            
            _context = new LaundryDbContext();
            _orderItems = new ObservableCollection<OrderItemViewModel>();
            OrderItemsDataGrid.ItemsSource = _orderItems;
            
            LoadCustomers();
            LoadServices();
        }
        
        private void LoadCustomers()
        {
            var customers = _context.Customers.OrderBy(c => c.Name).ToList();
            CustomerComboBox.ItemsSource = customers;
            
            CustomerComboBox.SelectionChanged += (s, e) => {
                var selectedCustomer = CustomerComboBox.SelectedItem as Customer;
                if (selectedCustomer != null)
                {
                    PhoneTextBox.Text = selectedCustomer.Phone;
                    AddressTextBox.Text = selectedCustomer.Address;
                }
                else
                {
                    PhoneTextBox.Text = string.Empty;
                    AddressTextBox.Text = string.Empty;
                }
            };
        }
        
        private void LoadServices()
        {
            var services = _context.Services.OrderBy(s => s.Name).ToList();
            ServiceComboBox.ItemsSource = services;
        }
        
        private void ServiceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedService = ServiceComboBox.SelectedItem as Service;
            if (selectedService != null)
            {
                PriceTextBox.Text = selectedService.PricePerKg.ToString("N0");
                CalculateSubtotal();
            }
            else
            {
                PriceTextBox.Text = string.Empty;
                SubtotalTextBox.Text = string.Empty;
            }
        }
        
        private void WeightTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateSubtotal();
        }
        
        private void CalculateSubtotal()
        {
            var selectedService = ServiceComboBox.SelectedItem as Service;
            if (selectedService != null && !string.IsNullOrWhiteSpace(WeightTextBox.Text))
            {
                if (decimal.TryParse(WeightTextBox.Text, out decimal weight))
                {
                    decimal subtotal = selectedService.PricePerKg * weight;
                    SubtotalTextBox.Text = subtotal.ToString("N0");
                }
                else
                {
                    SubtotalTextBox.Text = string.Empty;
                }
            }
        }
        
        private void AddServiceButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedService = ServiceComboBox.SelectedItem as Service;
            if (selectedService == null)
            {
                MessageBox.Show("Pilih layanan terlebih dahulu.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(WeightTextBox.Text) || !decimal.TryParse(WeightTextBox.Text, out decimal weight) || weight <= 0)
            {
                MessageBox.Show("Masukkan berat yang valid.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var orderItem = new OrderItemViewModel
            {
                No = _orderItems.Count + 1,
                ServiceId = selectedService.ServiceId,
                ServiceName = selectedService.Name,
                Weight = weight,
                UnitPrice = selectedService.PricePerKg,
                Subtotal = selectedService.PricePerKg * weight
            };
            
            _orderItems.Add(orderItem);
            
            // Reset the inputs
            ServiceComboBox.SelectedIndex = -1;
            WeightTextBox.Text = string.Empty;
            SubtotalTextBox.Text = string.Empty;
            
            UpdateOrderSummary();
        }
        
        private void RemoveServiceButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int itemNo = Convert.ToInt32(button.Tag);
            
            var itemToRemove = _orderItems.FirstOrDefault(i => i.No == itemNo);
            if (itemToRemove != null)
            {
                _orderItems.Remove(itemToRemove);
                
                // Renumber items
                for (int i = 0; i < _orderItems.Count; i++)
                {
                    _orderItems[i].No = i + 1;
                }
                
                UpdateOrderSummary();
            }
        }
        
        private void DiscountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(DiscountTextBox.Text, out decimal discount) && discount >= 0)
            {
                _discountAmount = discount;
            }
            else
            {
                _discountAmount = 0;
            }
            
            UpdateOrderSummary();
        }
        
        private void UpdateOrderSummary()
        {
            _totalAmount = _orderItems.Sum(i => i.Subtotal);
            _finalAmount = _totalAmount - _discountAmount;
            
            if (_finalAmount < 0)
            {
                _finalAmount = 0;
            }
            
            TotalAmountTextBlock.Text = _totalAmount.ToString("N0");
            FinalAmountTextBlock.Text = _finalAmount.ToString("N0");
        }
        
        private void NewCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            var customerWindow = new CustomerWindow();
            customerWindow.Owner = Window.GetWindow(this);
            customerWindow.ShowDialog();
            
            // Refresh customer list
            LoadCustomers();
        }
        
        private void SaveOrderButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate customer selection
            var selectedCustomer = CustomerComboBox.SelectedItem as Customer;
            if (selectedCustomer == null)
            {
                MessageBox.Show("Pilih pelanggan terlebih dahulu.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // Validate order items
            if (_orderItems.Count == 0)
            {
                MessageBox.Show("Tambahkan minimal satu layanan.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                // Create new order
                var order = new Order
                {
                    CustomerId = selectedCustomer.CustomerId,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.New,
                    TotalAmount = _totalAmount,
                    DiscountAmount = _discountAmount,
                    FinalAmount = _finalAmount,
                    IsPaid = false,
                    Notes = NotesTextBox.Text
                };
                
                _context.Orders.Add(order);
                _context.SaveChanges();
                
                // Add order items
                foreach (var item in _orderItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ServiceId = item.ServiceId,
                        Weight = item.Weight,
                        UnitPrice = item.UnitPrice,
                        Subtotal = item.Subtotal
                    };
                    
                    _context.OrderItems.Add(orderItem);
                }
                
                _context.SaveChanges();
                
                MessageBox.Show($"Pesanan berhasil disimpan dengan ID: {order.OrderId}", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Ask if user wants to make payment
                var result = MessageBox.Show("Apakah ingin melakukan pembayaran sekarang?", "Pembayaran", 
                                             MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var paymentWindow = new PaymentWindow(order.OrderId);
                    paymentWindow.Owner = Window.GetWindow(this);
                    paymentWindow.ShowDialog();
                }
                
                // Reset form
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Apakah yakin ingin membatalkan pesanan ini?", "Konfirmasi", 
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ResetForm();
            }
        }
        
        private void ResetForm()
        {
            CustomerComboBox.SelectedIndex = -1;
            ServiceComboBox.SelectedIndex = -1;
            WeightTextBox.Text = string.Empty;
            SubtotalTextBox.Text = string.Empty;
            DiscountTextBox.Text = string.Empty;
            NotesTextBox.Text = string.Empty;
            
            _orderItems.Clear();
            _totalAmount = 0;
            _discountAmount = 0;
            _finalAmount = 0;
            
            TotalAmountTextBlock.Text = "0";
            FinalAmountTextBlock.Text = "0";
        }
    }
}