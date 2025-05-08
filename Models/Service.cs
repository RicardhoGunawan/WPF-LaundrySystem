using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Laundry.Models
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        public decimal PricePerKg { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        public int EstimatedHours { get; set; }
        
        // Navigation property
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}