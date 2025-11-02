using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeSystem.Models
{
    /// <summary>
    /// Приход продуктов (накладные)
    /// </summary>
    public class ProductReceipt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime ReceiptDate { get; set; }

        [Required]
        [StringLength(30)]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(140)]
        public string SupplierName { get; set; } = string.Empty;

        [StringLength(9)]
        public string? SupplierUnp { get; set; }

        public DateTime? ContractDate { get; set; }

        [StringLength(30)]
        public string? ContractNumber { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Unit { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,3)")]
        public decimal Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}

