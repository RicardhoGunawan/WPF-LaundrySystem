// File: Laundry/Views/ServiceView.xaml.cs
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Laundry.Data;
using Laundry.Models;
using Microsoft.EntityFrameworkCore;

namespace Laundry.Views
{
    public partial class ServiceView : UserControl
    {
        private LaundryDbContext _context;
        private Service _currentService;
        private bool _isEditMode = false;
        
        public ServiceView()
        {
            InitializeComponent();
            
            _context = new LaundryDbContext();
            LoadServices();
            ClearForm();
        }
        
        private void LoadServices()
        {
            try
            {
                var services = _context.Services.OrderBy(s => s.Name).ToList();
                ServicesDataGrid.ItemsSource = services;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading services: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ClearForm()
        {
            _currentService = null;
            _isEditMode = false;
            
            ServiceIdTextBox.Text = string.Empty;
            NameTextBox.Text = string.Empty;
            PriceTextBox.Text = string.Empty;
            EstimatedHoursTextBox.Text = string.Empty;
            DescriptionTextBox.Text = string.Empty;
            
            DeleteButton.IsEnabled = false;
        }
        
        private void LoadServiceToForm(Service service)
        {
            if (service == null) return;
            
            _currentService = service;
            _isEditMode = true;
            
            ServiceIdTextBox.Text = service.ServiceId.ToString();
            NameTextBox.Text = service.Name;
            PriceTextBox.Text = service.PricePerKg.ToString("N0");
            EstimatedHoursTextBox.Text = service.EstimatedHours.ToString();
            DescriptionTextBox.Text = service.Description;
            
            DeleteButton.IsEnabled = true;
        }
        
        private void ServicesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedService = ServicesDataGrid.SelectedItem as Service;
            if (selectedService != null)
            {
                LoadServiceToForm(selectedService);
            }
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Nama layanan harus diisi.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(PriceTextBox.Text) || !decimal.TryParse(PriceTextBox.Text.Replace(",", ""), out decimal price) || price <= 0)
            {
                MessageBox.Show("Masukkan harga yang valid.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EstimatedHoursTextBox.Text) || !int.TryParse(EstimatedHoursTextBox.Text, out int hours) || hours <= 0)
            {
                MessageBox.Show("Masukkan estimasi jam yang valid.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                if (!_isEditMode)
                {
                    // Create new service
                    var newService = new Service
                    {
                        Name = NameTextBox.Text,
                        PricePerKg = price,
                        EstimatedHours = hours,
                        Description = DescriptionTextBox.Text
                    };
                    
                    _context.Services.Add(newService);
                    _context.SaveChanges();
                    
                    MessageBox.Show("Layanan baru berhasil disimpan.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Update existing service
                    _currentService.Name = NameTextBox.Text;
                    _currentService.PricePerKg = price;
                    _currentService.EstimatedHours = hours;
                    _currentService.Description = DescriptionTextBox.Text;
                    
                    _context.SaveChanges();
                    
                    MessageBox.Show("Layanan berhasil diupdate.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                LoadServices();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
        
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentService == null)
            {
                MessageBox.Show("Pilih layanan terlebih dahulu.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // Check if service is used in any order
            var isUsed = _context.OrderItems.Any(oi => oi.ServiceId == _currentService.ServiceId);
            if (isUsed)
            {
                MessageBox.Show("Tidak dapat menghapus layanan karena sudah digunakan dalam pesanan.",
                               "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var result = MessageBox.Show("Anda yakin ingin menghapus layanan ini?",
                                        "Konfirmasi", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Services.Remove(_currentService);
                    _context.SaveChanges();
                    
                    MessageBox.Show("Layanan berhasil dihapus.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    LoadServices();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}