using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeSystem.Models
{
    /// <summary>
    /// Ошибки импорта данных
    /// </summary>
    public class DataImportError
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ImportLogId { get; set; }

        [Required]
        public int RowNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string ErrorType { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string ErrorMessage { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? RowData { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ImportLogId")]
        public virtual DataImportLog ImportLog { get; set; } = null!;
    }
}

