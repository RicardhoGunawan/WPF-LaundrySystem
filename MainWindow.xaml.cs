using System;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace Laundry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
    
            // Tambahkan event handler untuk setiap RadioButton di NavPanel
            foreach (RadioButton rb in NavPanel.Children.OfType<RadioButton>())
            {
                rb.Checked += NavButton_Checked;
            }
        }

        private void NavButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.CommandParameter != null)
            {
                // Update judul halaman
                PageTitle.Text = rb.Tag.ToString();
        
                // Update konten
                ContentArea.Content = rb.CommandParameter;
            }
        }
    }
}