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

namespace Laundry.Windows
{
    public partial class OrderDetailWindow : Window
    {
        private LaundryDbContext _context;
        private int _orderId;
        private Order _order;
        
        public class OrderItemViewModel
        {
            public int No { get; set; }
            public string ServiceName { get; set; }
            public decimal Weight { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Subtotal { get; set; }
        }
        
        public OrderDetailWindow(int orderId)
        {
            InitializeComponent();
            _context = new LaundryDbContext();
            _orderId = orderId;
            
            LoadOrderDetails();
        }
        
        private void LoadOrderDetails()
        {
            try
            {
                // Perhatikan penggunaan Include yang tepat untuk memastikan OrderItems dan Service dimuat
                _order = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Service)
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
                CustomerPhoneTextBlock.Text = _order.Customer.Phone;
                StatusTextBlock.Text = _order.Status.ToString();
                PaymentStatusTextBlock.Text = _order.IsPaid ? "Sudah Dibayar" : "Belum Dibayar";
                
                // Tambahkan pengecekan dan debugging
                if (_order.OrderItems == null || !_order.OrderItems.Any())
                {
                    MessageBox.Show("Tidak ada item pesanan yang ditemukan.", "Informasi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Debug: Tampilkan jumlah item yang ditemukan
                    Console.WriteLine($"Jumlah item pesanan: {_order.OrderItems.Count}");
                    
                    var orderItems = _order.OrderItems.Select((item, index) => new OrderItemViewModel
                    {
                        No = index + 1,
                        ServiceName = item.Service?.Name ?? "Layanan tidak tersedia",
                        Weight = item.Weight,
                        UnitPrice = item.UnitPrice,
                        Subtotal = item.Subtotal
                    }).ToList();
                    
                    // Pastikan untuk menetapkan ItemsSource ke DataGrid
                    OrderItemsDataGrid.ItemsSource = orderItems;
                }
                
                TotalAmountTextBlock.Text = _order.TotalAmount.ToString("N0");
                DiscountTextBlock.Text = (_order.DiscountAmount ?? 0).ToString("N0");
                FinalAmountTextBlock.Text = _order.FinalAmount.ToString("N0");
                
                NotesTextBox.Text = _order.Notes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading order details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Tambahkan debugging lebih detail
                Console.WriteLine($"Detail error: {ex.ToString()}");
            }
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
                    printDialog.PrintDocument(idpSource.DocumentPaginator, "Pesanan Laundry #" + _orderId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing: {ex.Message}", "Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private FlowDocument CreatePrintContent()
        {
            var doc = new FlowDocument();
            doc.FontFamily = new FontFamily("Arial");
            
            var headerPara = new Paragraph(new Run("STRUK LAUNDRY"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(headerPara);
            
            var datePara = new Paragraph(new Run(DateTime.Now.ToString("dd/MM/yyyy HH:mm")))
            {
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(datePara);
            
            var separatorPara = new Paragraph(new Run("=".PadRight(40, '=')))
            {
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(separatorPara);
            
            var orderInfoPara = new Paragraph();
            orderInfoPara.Inlines.Add(new Run("No. Pesanan: ") { FontWeight = FontWeights.Bold });
            orderInfoPara.Inlines.Add(new Run(_order.OrderId.ToString()));
            orderInfoPara.Inlines.Add(new LineBreak());
            
            orderInfoPara.Inlines.Add(new Run("Tanggal: ") { FontWeight = FontWeights.Bold });
            orderInfoPara.Inlines.Add(new Run(_order.OrderDate.ToString("dd/MM/yyyy HH:mm")));
            orderInfoPara.Inlines.Add(new LineBreak());
            
            orderInfoPara.Inlines.Add(new Run("Pelanggan: ") { FontWeight = FontWeights.Bold });
            orderInfoPara.Inlines.Add(new Run(_order.Customer.Name));
            orderInfoPara.Inlines.Add(new LineBreak());
            
            orderInfoPara.Inlines.Add(new Run("Status: ") { FontWeight = FontWeights.Bold });
            orderInfoPara.Inlines.Add(new Run(_order.Status.ToString()));
            
            doc.Blocks.Add(orderInfoPara);
            
            doc.Blocks.Add(new Paragraph(new Run("-".PadRight(40, '-'))));
            
            var itemsHeaderPara = new Paragraph(new Run("ITEM PESANAN"))
            {
                FontWeight = FontWeights.Bold
            };
            doc.Blocks.Add(itemsHeaderPara);
            
            foreach (var item in _order.OrderItems)
            {
                var itemPara = new Paragraph();
                itemPara.Inlines.Add(new Run(item.Service.Name));
                itemPara.Inlines.Add(new LineBreak());
                itemPara.Inlines.Add(new Run($"{item.Weight} kg x Rp {item.UnitPrice:N0} = Rp {item.Subtotal:N0}"));
                
                doc.Blocks.Add(itemPara);
            }
            
            doc.Blocks.Add(new Paragraph(new Run("-".PadRight(40, '-'))));
            
            var summaryPara = new Paragraph
            {
                TextAlignment = TextAlignment.Right
            };

            summaryPara.Inlines.Add(new Run("Total: Rp " + _order.TotalAmount.ToString("N0")));
            summaryPara.Inlines.Add(new LineBreak());

            summaryPara.Inlines.Add(new Run("Diskon: Rp " + (_order.DiscountAmount ?? 0).ToString("N0")));
            summaryPara.Inlines.Add(new LineBreak());

            summaryPara.Inlines.Add(new Run("Total Akhir: Rp " + _order.FinalAmount.ToString("N0"))
            {
                FontWeight = FontWeights.Bold
            });

            doc.Blocks.Add(summaryPara);

            
            summaryPara.Inlines.Add(new Run("Total Akhir: ") { FontWeight = FontWeights.Bold });
            
            doc.Blocks.Add(summaryPara);
            
            doc.Blocks.Add(new Paragraph(new Run("=".PadRight(40, '='))));
            
            var footerPara = new Paragraph(new Run("Terima kasih telah menggunakan jasa laundry kami!"))
            {
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(footerPara);
            
            return doc;
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}