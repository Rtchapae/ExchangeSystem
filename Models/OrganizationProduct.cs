using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeSystem.Models
{
    /// <summary>
    /// Связь продукта с организацией и код СВС для конкретной организации
    /// Один и тот же продукт может иметь разные коды СВС в разных организациях
    /// </summary>
    public class OrganizationProduct
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public int ProductId { get; set; }

        /// <summary>
        /// Код продукта в системе СВС для этой организации
        /// </summary>
        [StringLength(50)]
        public string? SvsCode { get; set; }

        /// <summary>
        /// Локальная цена для этой организации (если отличается)
        /// </summary>
        public decimal? LocalPrice { get; set; }

        /// <summary>
        /// Активен ли продукт для этой организации
        /// </summary>
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("OrganizationId")]
        public virtual Store Organization { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}


