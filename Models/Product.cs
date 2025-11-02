using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeSystem.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Category { get; set; }

        /// <summary>
        /// Код продукта (Шифр из системы питания, например 610001)
        /// </summary>
        [StringLength(20)]
        public string? Code { get; set; }

        /// <summary>
        /// Внутренний ID из системы питания (например 213|13, 88|6)
        /// </summary>
        [StringLength(50)]
        public string? ExternalId { get; set; }

        /// <summary>
        /// Код продукта в системе СВС (ваш код для сопоставления)
        /// </summary>
        [StringLength(50)]
        public string? SvsCode { get; set; }

        [StringLength(100)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Unit { get; set; }

        public decimal? Price { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}


