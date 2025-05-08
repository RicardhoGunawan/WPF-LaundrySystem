using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laundry.Models
{
    public enum OrderStatus
    {
        New,
        Processing,
        ReadyForPickup,
        Completed,
        Cancelled
    }
    
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        
        public int CustomerId { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; }
        
        public DateTime? DeliveryDate { get; set; }
        
        [Required]
        public OrderStatus Status { get; set; }
        
        [Required]
        public decimal TotalAmount { get; set; }
        
        public decimal? DiscountAmount { get; set; }
        
        public decimal FinalAmount { get; set; }
        
        public bool IsPaid { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; } 
        
        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}