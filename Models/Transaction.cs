using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeSystem.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int StoreId { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [StringLength(50)]
        public string? DocumentNumber { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal TotalAmount => Quantity * Price;

        [StringLength(100)]
        public string? Supplier { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [ForeignKey("StoreId")]
        public virtual Store Store { get; set; } = null!;
    }
}


