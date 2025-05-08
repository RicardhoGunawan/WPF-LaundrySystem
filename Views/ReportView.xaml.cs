using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Laundry.Data;
using Laundry.Models;
using Microsoft.EntityFrameworkCore;

namespace Laundry.Views
{
    public partial class ReportView : UserControl
    {
        private LaundryDbContext _context;
        private DateTime _startDate;
        private DateTime _endDate;
        
        public class DailySummary
        {
            public DateTime Date { get; set; }
            public int OrderCount { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal AverageAmount { get; set; }
        }
        
        public class ServiceReport
        {
            public string ServiceName { get; set; }
            public int OrderCount { get; set; }
            public decimal TotalWeight { get; set; }
            public decimal TotalRevenue { get; set; }
            public double Percentage { get; set; }
        }
        
        public class CustomerReport
        {
            public string CustomerName { get; set; }
            public int OrderCount { get; set; }
            public decimal TotalSpent { get; set; }
            public decimal AveragePerOrder { get; set; }
            public DateTime LastOrderDate { get; set; }
        }
        
        public ReportView()
        {
            InitializeComponent();
            
            _context = new LaundryDbContext();
            
            // Setup period filter
            var periods = new List<string>
            {
                "Hari Ini",
                "Minggu Ini",
                "Bulan Ini",
                "Tahun Ini",
                "Kustom"
            };
            PeriodComboBox.ItemsSource = periods;
            PeriodComboBox.SelectedIndex = 2; // Default to "Bulan Ini"
            
            // Setup date pickers
            StartDatePicker.SelectedDate = DateTime.Now.AddDays(-30);
            EndDatePicker.SelectedDate = DateTime.Now;
            
            UpdateDateRange();
        }
        
        private void UpdateDateRange()
        {
            var selectedPeriod = PeriodComboBox.SelectedItem as string;
            
            switch (selectedPeriod)
            {
                case "Hari Ini":
                    _startDate = DateTime.Today;
                    _endDate = DateTime.Today.AddDays(1).AddSeconds(-1);
                    break;
                    
                case "Minggu Ini":
                    // Monday as first day of week
                    int diff = (7 + (DateTime.Today.DayOfWeek - DayOfWeek.Monday)) % 7;
                    _startDate = DateTime.Today.AddDays(-diff);
                    _endDate = _startDate.AddDays(7).AddSeconds(-1);
                    break;
                    
                case "Bulan Ini":
                    _startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    _endDate = _startDate.AddMonths(1).AddSeconds(-1);
                    break;
                    
                case "Tahun Ini":
                    _startDate = new DateTime(DateTime.Today.Year, 1, 1);
                    _endDate = _startDate.AddYears(1).AddSeconds(-1);
                    break;
                    
                case "Kustom":
                    _startDate = StartDatePicker.SelectedDate ?? DateTime.Today.AddDays(-30);
                    _endDate = EndDatePicker.SelectedDate ?? DateTime.Today;
                    _endDate = _endDate.AddDays(1).AddSeconds(-1); // Include the end date
                    break;
            }
            
            StartDatePicker.IsEnabled = selectedPeriod == "Kustom";
            EndDatePicker.IsEnabled = selectedPeriod == "Kustom";
            
            if (selectedPeriod != "Kustom")
            {
                StartDatePicker.SelectedDate = _startDate;
                EndDatePicker.SelectedDate = _endDate;
            }
        }
        
        private void PeriodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDateRange();
        }
        
        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateDateRange();
                GenerateSummaryReport();
                GenerateServiceReport();
                GenerateCustomerReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void GenerateSummaryReport()
        {
            var orders = _context.Orders
                .Where(o => o.OrderDate >= _startDate && o.OrderDate <= _endDate)
                .Include(o => o.Customer)
                .ToList();
            
            // Update summary cards
            TotalOrdersTextBlock.Text = orders.Count.ToString();
            TotalRevenueTextBlock.Text = $"Rp {orders.Sum(o => o.FinalAmount):N0}";
            UnpaidOrdersTextBlock.Text = orders.Count(o => !o.IsPaid).ToString();
            CompletedOrdersTextBlock.Text = orders.Count(o => o.Status == OrderStatus.Completed).ToString();
            
            // Group by date for daily summary
            var dailySummary = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new DailySummary
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    TotalAmount = g.Sum(o => o.FinalAmount),
                    AverageAmount = g.Average(o => o.FinalAmount)
                })
                .OrderByDescending(s => s.Date)
                .ToList();
            
            SummaryDataGrid.ItemsSource = dailySummary;
        }
        
        private void GenerateServiceReport()
        {
            var orderItems = _context.OrderItems
                .Where(oi => oi.Order.OrderDate >= _startDate && oi.Order.OrderDate <= _endDate)
                .Include(oi => oi.Service)
                .Include(oi => oi.Order)
                .ToList();
            
            var totalRevenue = orderItems.Sum(oi => oi.Subtotal);
            
            var serviceReport = orderItems
                .GroupBy(oi => oi.ServiceId)
                .Select(g => new ServiceReport
                {
                    ServiceName = g.First().Service.Name,
                    OrderCount = g.Count(),
                    TotalWeight = g.Sum(oi => oi.Weight),
                    TotalRevenue = g.Sum(oi => oi.Subtotal),
                    Percentage = totalRevenue == 0 ? 0 : (double)(g.Sum(oi => oi.Subtotal) / totalRevenue)
                })
                .OrderByDescending(s => s.TotalRevenue)
                .ToList();
            
            ServiceReportDataGrid.ItemsSource = serviceReport;
        }
        
        private void GenerateCustomerReport()
        {
            var orders = _context.Orders
                .Where(o => o.OrderDate >= _startDate && o.OrderDate <= _endDate)
                .Include(o => o.Customer)
                .ToList();
            
            var customerReport = orders
                .GroupBy(o => o.CustomerId)
                .Select(g => new CustomerReport
                {
                    CustomerName = g.First().Customer.Name,
                    OrderCount = g.Count(),
                    TotalSpent = g.Sum(o => o.FinalAmount),
                    AveragePerOrder = g.Average(o => o.FinalAmount),
                    LastOrderDate = g.Max(o => o.OrderDate)
                })
                .OrderByDescending(c => c.TotalSpent)
                .ToList();
            
            CustomerReportDataGrid.ItemsSource = customerReport;
        }
        
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fitur export belum diimplementasikan.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}