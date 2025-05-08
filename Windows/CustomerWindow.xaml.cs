using System;
using System.Windows;
using Laundry.Data;
using Laundry.Models;

namespace Laundry.Windows
{
    public partial class CustomerWindow : Window
    {
        private LaundryDbContext _context;
        private Customer _customer;
        private bool _isEdit;
        
        public CustomerWindow()
        {
            InitializeComponent();
            _context = new LaundryDbContext();
            _customer = new Customer();
            _isEdit = false;
        }
        
        public CustomerWindow(int customerId) : this()
        {
            _customer = _context.Customers.Find(customerId);
            if (_customer != null)
            {
                _isEdit = true;
                Title = "Edit Pelanggan";
                LoadCustomerData();
            }
        }
        
        private void LoadCustomerData()
        {
            NameTextBox.Text = _customer.Name;
            PhoneTextBox.Text = _customer.Phone;
            AddressTextBox.Text = _customer.Address;
            EmailTextBox.Text = _customer.Email;
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Nama pelanggan harus diisi.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                _customer.Name = NameTextBox.Text;
                _customer.Phone = PhoneTextBox.Text;
                _customer.Address = AddressTextBox.Text;
                _customer.Email = EmailTextBox.Text;
                
                if (!_isEdit)
                {
                    _customer.RegisterDate = DateTime.Now;
                    _context.Customers.Add(_customer);
                }
                
                _context.SaveChanges();
                MessageBox.Show("Data pelanggan berhasil disimpan.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
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