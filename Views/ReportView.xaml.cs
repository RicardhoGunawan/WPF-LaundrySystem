using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Printing;
using Laundry.Data;
using Laundry.Models;
using Microsoft.EntityFrameworkCore;

namespace Laundry.Views
{
    public partial class ReportView : UserControl
    {
        private LaundryDbContext _context;
        
        public class SalesReportItem
        {
            public DateTime Date { get; set; }
            public int OrderCount { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal PaidAmount { get; set; }
            public decimal UnpaidAmount { get; set; }
        }
        
        public class ServiceReportItem
        {
            public string ServiceName { get; set; }
            public int OrderCount { get; set; }
            public decimal TotalWeight { get; set; }
            public decimal TotalAmount { get; set; }
            public double Percentage { get; set; }
        }
        
        public class TopCustomerItem
        {
            public string CustomerName { get; set; }
            public int OrderCount { get; set; }
            public decimal TotalAmount { get; set; }
            public DateTime LastOrder { get; set; }
        }
        
        public ReportView()
        {
            InitializeComponent();
            
            _context = new LaundryDbContext();
            
            // Set default date range to current month
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            
            StartDatePicker.SelectedDate = firstDayOfMonth;
            EndDatePicker.SelectedDate = today;
            
            GenerateReport();
        }
        
        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateReport();
        }
        
        private void GenerateReport()
        {
            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Pilih rentang tanggal yang valid.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var startDate = StartDatePicker.SelectedDate.Value;
            var endDate = EndDatePicker.SelectedDate.Value.AddDays(1).AddSeconds(-1); // End of the selected day
            
            if (startDate > endDate)
            {
                MessageBox.Show("Tanggal awal harus lebih kecil dari tanggal akhir.", "Peringatan", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                GenerateSalesReport(startDate, endDate);
                GenerateServiceReport(startDate, endDate);
                GenerateTopCustomersReport(startDate, endDate);
                UpdateSummary(startDate, endDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void GenerateSalesReport(DateTime startDate, DateTime endDate)
        {
            var orders = _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToList();
            
            var salesReport = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new SalesReportItem
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    TotalAmount = g.Sum(o => o.FinalAmount),
                    PaidAmount = g.Where(o => o.IsPaid).Sum(o => o.FinalAmount),
                    UnpaidAmount = g.Where(o => !o.IsPaid).Sum(o => o.FinalAmount)
                })
                .OrderBy(s => s.Date)
                .ToList();
            
            SalesDataGrid.ItemsSource = salesReport;
        }
        
        private void GenerateServiceReport(DateTime startDate, DateTime endDate)
        {
            var orderItems = _context.OrderItems
                .Include(oi => oi.Service)
                .Include(oi => oi.Order)
                .Where(oi => oi.Order.OrderDate >= startDate && oi.Order.OrderDate <= endDate)
                .ToList();
            
            decimal totalAmount = orderItems.Sum(oi => oi.Subtotal);
            
            var serviceReport = orderItems
                .GroupBy(oi => oi.Service.Name)
                .Select(g => new ServiceReportItem
                {
                    ServiceName = g.Key,
                    OrderCount = g.Select(oi => oi.OrderId).Distinct().Count(),
                    TotalWeight = g.Sum(oi => oi.Weight),
                    TotalAmount = g.Sum(oi => oi.Subtotal),
                    Percentage = totalAmount > 0 ? (double)(g.Sum(oi => oi.Subtotal) / totalAmount * 100) : 0
                })
                .OrderByDescending(s => s.TotalAmount)
                .ToList();
            
            ServiceReportDataGrid.ItemsSource = serviceReport;
        }
        
        private void GenerateTopCustomersReport(DateTime startDate, DateTime endDate)
        {
            var topCustomers = _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => new { o.CustomerId, o.Customer.Name })
                .Select(g => new TopCustomerItem
                {
                    CustomerName = g.Key.Name,
                    OrderCount = g.Count(),
                    TotalAmount = g.Sum(o => o.FinalAmount),
                    LastOrder = g.Max(o => o.OrderDate)
                })
                .OrderByDescending(c => c.TotalAmount)
                .Take(10)
                .ToList();
            
            TopCustomersDataGrid.ItemsSource = topCustomers;
        }
        
        private void UpdateSummary(DateTime startDate, DateTime endDate)
        {
            var orders = _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToList();
            
            int totalOrders = orders.Count;
            decimal totalIncome = orders.Sum(o => o.FinalAmount);
            decimal averagePerOrder = totalOrders > 0 ? totalIncome / totalOrders : 0;
            
            TotalOrdersTextBlock.Text = totalOrders.ToString();
            TotalIncomeTextBlock.Text = $"Rp {totalIncome:N0}";
            AveragePerOrderTextBlock.Text = $"Rp {averagePerOrder:N0}";
        }
        
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    var documentToPrint = CreatePrintContent();
                    IDocumentPaginatorSource idpSource = documentToPrint;
                    printDialog.PrintDocument(idpSource.DocumentPaginator, "Laporan Laundry");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing: {ex.Message}", "Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private FlowDocument CreatePrintContent()
        {
            var startDate = StartDatePicker.SelectedDate.Value;
            var endDate = EndDatePicker.SelectedDate.Value;
            
            var doc = new FlowDocument();
            doc.FontFamily = new FontFamily("Arial");
            
            var headerPara = new Paragraph(new Run("LAPORAN LAUNDRY"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(headerPara);
            
            var datePara = new Paragraph(new Run($"Periode: {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}"))
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };
            doc.Blocks.Add(datePara);
            
            var summaryPara = new Paragraph();
            summaryPara.Inlines.Add(new Run("RINGKASAN") { FontWeight = FontWeights.Bold });
            doc.Blocks.Add(summaryPara);
            
            var summaryTablePara = new Paragraph();
            summaryTablePara.Inlines.Add(new Run("Total Pesanan: ") { FontWeight = FontWeights.Bold });
            summaryTablePara.Inlines.Add(new Run(TotalOrdersTextBlock.Text));
            summaryTablePara.Inlines.Add(new LineBreak());
            
            summaryTablePara.Inlines.Add(new Run("Total Pendapatan: ") { FontWeight = FontWeights.Bold });
            summaryTablePara.Inlines.Add(new Run(TotalIncomeTextBlock.Text));
            summaryTablePara.Inlines.Add(new LineBreak());
            
            summaryTablePara.Inlines.Add(new Run("Rata-rata per Pesanan: ") { FontWeight = FontWeights.Bold });
            summaryTablePara.Inlines.Add(new Run(AveragePerOrderTextBlock.Text));
            
            doc.Blocks.Add(summaryTablePara);
            doc.Blocks.Add(new Paragraph(new Run(" ")));
            
            var salesHeaderPara = new Paragraph();
            salesHeaderPara.Inlines.Add(new Run("LAPORAN PENJUALAN HARIAN") { FontWeight = FontWeights.Bold });
            doc.Blocks.Add(salesHeaderPara);
            
            var salesTable = new Table();
            
            for (int i = 0; i < 5; i++)
                salesTable.Columns.Add(new TableColumn());
            
            var salesHeaderRow = new TableRow();
            salesHeaderRow.Cells.Add(CreateTableCell("Tanggal", true));
            salesHeaderRow.Cells.Add(CreateTableCell("Jml Pesanan", true));
            salesHeaderRow.Cells.Add(CreateTableCell("Total", true));
            salesHeaderRow.Cells.Add(CreateTableCell("Dibayar", true));
            salesHeaderRow.Cells.Add(CreateTableCell("Pending", true));
            salesTable.RowGroups.Add(new TableRowGroup());
            salesTable.RowGroups[0].Rows.Add(salesHeaderRow);
            
            var salesItems = SalesDataGrid.ItemsSource as List<SalesReportItem>;
            if (salesItems != null)
            {
                foreach (var item in salesItems)
                {
                    var dataRow = new TableRow();
                    dataRow.Cells.Add(CreateTableCell(item.Date.ToString("dd/MM/yyyy")));
                    dataRow.Cells.Add(CreateTableCell(item.OrderCount.ToString()));
                    dataRow.Cells.Add(CreateTableCell($"Rp {item.TotalAmount:N0}"));
                    dataRow.Cells.Add(CreateTableCell($"Rp {item.PaidAmount:N0}"));
                    dataRow.Cells.Add(CreateTableCell($"Rp {item.UnpaidAmount:N0}"));
                    salesTable.RowGroups[0].Rows.Add(dataRow);
                }
            }
            
            doc.Blocks.Add(salesTable);
            
            doc.Blocks.Add(new Paragraph(new Run(" ")) { BreakPageBefore = true });
            
            var serviceHeaderPara = new Paragraph();
            serviceHeaderPara.Inlines.Add(new Run("LAPORAN LAYANAN") { FontWeight = FontWeights.Bold });
            doc.Blocks.Add(serviceHeaderPara);
            
            var serviceTable = new Table();
            
            for (int i = 0; i < 5; i++)
                serviceTable.Columns.Add(new TableColumn());
            
            var serviceHeaderRow = new TableRow();
            serviceHeaderRow.Cells.Add(CreateTableCell("Layanan", true));
            serviceHeaderRow.Cells.Add(CreateTableCell("Jml Pesanan", true));
            serviceHeaderRow.Cells.Add(CreateTableCell("Total Kg", true));
            serviceHeaderRow.Cells.Add(CreateTableCell("Total Pendapatan", true));
            serviceHeaderRow.Cells.Add(CreateTableCell("% dari Total", true));
            serviceTable.RowGroups.Add(new TableRowGroup());
            serviceTable.RowGroups[0].Rows.Add(serviceHeaderRow);
            
            var serviceItems = ServiceReportDataGrid.ItemsSource as List<ServiceReportItem>;
            if (serviceItems != null)
            {
                foreach (var item in serviceItems)
                {
                    var dataRow = new TableRow();
                    dataRow.Cells.Add(CreateTableCell(item.ServiceName));
                    dataRow.Cells.Add(CreateTableCell(item.OrderCount.ToString()));
                    dataRow.Cells.Add(CreateTableCell(item.TotalWeight.ToString("0.00")));
                    dataRow.Cells.Add(CreateTableCell($"Rp {item.TotalAmount:N0}"));
                    dataRow.Cells.Add(CreateTableCell($"{item.Percentage:0.00}%"));
                    serviceTable.RowGroups[0].Rows.Add(dataRow);
                }
            }
            
            doc.Blocks.Add(serviceTable);
            
            doc.Blocks.Add(new Paragraph(new Run(" ")) { BreakPageBefore = true });
            
            var customersHeaderPara = new Paragraph();
            customersHeaderPara.Inlines.Add(new Run("PELANGGAN TERATAS") { FontWeight = FontWeights.Bold });
            doc.Blocks.Add(customersHeaderPara);
            
            var customersTable = new Table();
            
            for (int i = 0; i < 4; i++)
                customersTable.Columns.Add(new TableColumn());
            
            var customersHeaderRow = new TableRow();
            customersHeaderRow.Cells.Add(CreateTableCell("Pelanggan", true));
            customersHeaderRow.Cells.Add(CreateTableCell("Jml Pesanan", true));
            customersHeaderRow.Cells.Add(CreateTableCell("Total Pengeluaran", true));
            customersHeaderRow.Cells.Add(CreateTableCell("Pesanan Terakhir", true));
            customersTable.RowGroups.Add(new TableRowGroup());
            customersTable.RowGroups[0].Rows.Add(customersHeaderRow);
            
            var customerItems = TopCustomersDataGrid.ItemsSource as List<TopCustomerItem>;
            if (customerItems != null)
            {
                foreach (var item in customerItems)
                {
                    var dataRow = new TableRow();
                    dataRow.Cells.Add(CreateTableCell(item.CustomerName));
                    dataRow.Cells.Add(CreateTableCell(item.OrderCount.ToString()));
                    dataRow.Cells.Add(CreateTableCell($"Rp {item.TotalAmount:N0}"));
                    dataRow.Cells.Add(CreateTableCell(item.LastOrder.ToString("dd/MM/yyyy")));
                    customersTable.RowGroups[0].Rows.Add(dataRow);
                }
            }
            
            doc.Blocks.Add(customersTable);
            
            return doc;
        }
        
        private TableCell CreateTableCell(string text, bool isHeader = false)
        {
            var cell = new TableCell(new Paragraph(new Run(text)));
            cell.BorderThickness = new Thickness(1);
            cell.BorderBrush = Brushes.Gray;
            cell.Padding = new Thickness(3);
            
            if (isHeader)
            {
                cell.Background = Brushes.LightGray;
                cell.FontWeight = FontWeights.Bold;
            }
            
            return cell;
        }
    }
}