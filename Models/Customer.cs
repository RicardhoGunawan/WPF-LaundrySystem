using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laundry.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [StringLength(20)]
        public string Phone { get; set; }
        
        [StringLength(200)]
        public string Address { get; set; }
        
        [StringLength(100)]
        public string Email { get; set; }
        
        public DateTime RegisterDate { get; set; }
        
        // Navigation property
        public virtual ICollection<Order> Orders { get; set; }
    }
}