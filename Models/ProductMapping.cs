using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeSystem.Models
{
    /// <summary>
    /// Сопоставление продуктов между ПО по учёту питания и СВС
    /// </summary>
    public class ProductMapping
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ExternalProductCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ExternalProductName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ExternalUnit { get; set; }

        [StringLength(50)]
        public string? ExternalGroup { get; set; }

        [StringLength(30)]
        public string? ContractNumber { get; set; }

        // Связь с продуктом в СВС
        public int? SvsProductId { get; set; }

        [StringLength(100)]
        public string? SvsProductName { get; set; }

        [StringLength(20)]
        public string? SvsUnit { get; set; }

        [StringLength(50)]
        public string? SvsGroup { get; set; }

        public bool IsAutoMapped { get; set; } = false;

        public bool IsApproved { get; set; } = false;

        [StringLength(200)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedAt { get; set; }

        // Navigation properties
        [ForeignKey("SvsProductId")]
        public virtual Product? SvsProduct { get; set; }
    }
}

