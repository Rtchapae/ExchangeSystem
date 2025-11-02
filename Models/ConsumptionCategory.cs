using System.ComponentModel.DataAnnotations;

namespace ExchangeSystem.Models
{
    /// <summary>
    /// Категории питающихся
    /// </summary>
    public class ConsumptionCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        [StringLength(20)]
        public string? Code { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

