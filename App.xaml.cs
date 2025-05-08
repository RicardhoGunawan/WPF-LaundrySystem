using System;
using System.Windows;
using Laundry.Data;

namespace Laundry
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Ensure database is created
            using (var context = new LaundryDbContext())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}