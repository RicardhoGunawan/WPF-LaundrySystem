using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laundry.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        
        public int OrderId { get; set; }
        
        public int ServiceId { get; set; }
        
        [Required]
        public decimal Weight { get; set; }
        
        [Required]
        public decimal UnitPrice { get; set; }
        
        [Required]
        public decimal Subtotal { get; set; }
        
        [StringLength(200)]
        public string Notes { get; set; }
        
        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
        
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
    }
}