using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeSystem.Models
{
    /// <summary>
    /// Расход продуктов по категориям питающихся
    /// </summary>
    public class ProductConsumption
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime ConsumptionDate { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Unit { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,3)")]
        public decimal Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        // Ссылки на организацию и УО
        public int? OrganizationId { get; set; }

        public int? EducationDepartmentId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; } = string.Empty;

        // Детализация по категориям
        [Column(TypeName = "decimal(15,3)")]
        public decimal? NurseryQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? NurseryCost { get; set; }

        [Column(TypeName = "decimal(15,3)")]
        public decimal? KindergartenQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? KindergartenCost { get; set; }

        [Column(TypeName = "decimal(15,3)")]
        public decimal? StaffQuantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? StaffCost { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [ForeignKey("CategoryId")]
        public virtual ConsumptionCategory Category { get; set; } = null!;

        [ForeignKey("OrganizationId")]
        public virtual Store? Organization { get; set; }
    }
}

