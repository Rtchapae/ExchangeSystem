using System.ComponentModel.DataAnnotations;

namespace ExchangeSystem.Models
{
    /// <summary>
    /// Лог импорта данных
    /// </summary>
    public class DataImportLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ImportType { get; set; } = string.Empty; // "Products", "Consumption", "Receipts"

        [Required]
        public DateTime ImportDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int TotalRecords { get; set; }

        [Required]
        public int ProcessedRecords { get; set; }

        [Required]
        public int SuccessRecords { get; set; }

        [Required]
        public int ErrorRecords { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Processing"; // "Processing", "Completed", "Failed"

        [StringLength(1000)]
        public string? ErrorMessage { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        public virtual ICollection<DataImportError> Errors { get; set; } = new List<DataImportError>();
    }
}

