using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laundry.Models
{
    public enum PaymentMethod
    {
        Cash,
        CreditCard,
        DebitCard,
        Transfer,
        Digital
    }
    
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        
        public int OrderId { get; set; }
        
        [Required]
        public DateTime PaymentDate { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public PaymentMethod Method { get; set; }
        
        [StringLength(100)]
        public string ReferenceNumber { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; }
        
        // Navigation property
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }
}