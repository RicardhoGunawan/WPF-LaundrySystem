using Microsoft.EntityFrameworkCore;
using Laundry.Models;
using System;
using System.IO;

namespace Laundry.Data
{
    public class LaundryDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LaundryApp", "laundry.db");
            
            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
            
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Seed data for services
            modelBuilder.Entity<Service>().HasData(
                new Service { ServiceId = 1, Name = "Cuci Reguler", PricePerKg = 7000, Description = "Cuci dan setrika reguler", EstimatedHours = 24 },
                new Service { ServiceId = 2, Name = "Cuci Express", PricePerKg = 12000, Description = "Cuci dan setrika express (6 jam)", EstimatedHours = 6 },
                new Service { ServiceId = 3, Name = "Dry Clean", PricePerKg = 15000, Description = "Dry cleaning untuk pakaian khusus", EstimatedHours = 48 },
                new Service { ServiceId = 4, Name = "Cuci Sepatu", PricePerKg = 25000, Description = "Cuci sepatu", EstimatedHours = 24 }
            );
        }
    }
}