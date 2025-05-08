using System;
using System.Windows;
using Laundry.Data;
using Laundry.Models;

namespace Laundry.Windows
{
    public partial class ServiceWindow : Window
    {
        private LaundryDbContext _context;
        private Service _service;
        private bool _isEdit;
        
        public ServiceWindow()
        {
            InitializeComponent();
            _context = new LaundryDbContext();
            _service = new Service();
            _isEdit = false;
        }
        
        public ServiceWindow(int serviceId) : this()
        {
            _service = _context.Services.Find(serviceId);
            if (_service != null)
            {
                _isEdit = true;
                Title = "Edit Layanan";
                LoadServiceData();
            }
        }
        
        private void LoadServiceData()
        {
            NameTextBox.Text = _service.Name;
            PriceTextBox.Text = _service.PricePerKg.ToString();
            HoursTextBox.Text = _service.EstimatedHours.ToString();
            DescriptionTextBox.Text = _service.Description;
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Nama layanan harus diisi.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Masukkan harga yang valid.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (!int.TryParse(HoursTextBox.Text, out int hours) || hours <= 0)
            {
                MessageBox.Show("Masukkan estimasi jam yang valid.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                _service.Name = NameTextBox.Text;
                _service.PricePerKg = price;
                _service.EstimatedHours = hours;
                _service.Description = DescriptionTextBox.Text;
                
                if (!_isEdit)
                {
                    _context.Services.Add(_service);
                }
                
                _context.SaveChanges();
                MessageBox.Show("Data layanan berhasil disimpan.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
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