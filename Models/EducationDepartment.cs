using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeSystem.Models
{
    /// <summary>
    /// Управление образования (УО)
    /// </summary>
    public class EducationDepartment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? DirectorName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<Store> Organizations { get; set; } = new List<Store>();
    }

    /// <summary>
    /// Обновление справочника СВС
    /// </summary>
    public class SvsCatalogUpdate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int TotalMaterials { get; set; }

        [Required]
        public int MappedMaterials { get; set; }

        [Required]
        public int UnmappedMaterials { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(50)]
        public string? UpdateSource { get; set; } // "Manual", "API", "File"

        public int? EducationDepartmentId { get; set; }

        [ForeignKey("EducationDepartmentId")]
        public virtual EducationDepartment? EducationDepartment { get; set; }

        // Navigation properties
        public virtual ICollection<SvsMaterialMapping> MaterialMappings { get; set; } = new List<SvsMaterialMapping>();
    }

    /// <summary>
    /// Сопоставление материалов СВС с продуктами
    /// </summary>
    public class SvsMaterialMapping
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SvsMatId { get; set; }

        [Required]
        [StringLength(200)]
        public string SvsMaterialName { get; set; } = string.Empty;

        [Required]
        public int SvsGroupId { get; set; }

        [Required]
        [StringLength(100)]
        public string SvsGroupName { get; set; } = string.Empty;

        [StringLength(50)]
        public string SvsMeasure { get; set; } = string.Empty;

        public int? ProductId { get; set; }

        [StringLength(50)]
        public string? SvsCode { get; set; }

        public bool IsAutoMapped { get; set; }

        public double Confidence { get; set; }

        [StringLength(500)]
        public string? MappingNotes { get; set; }

        [Required]
        public int CatalogUpdateId { get; set; }

        public int? EducationDepartmentId { get; set; }

        public int? OrganizationId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("CatalogUpdateId")]
        public virtual SvsCatalogUpdate CatalogUpdate { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [ForeignKey("EducationDepartmentId")]
        public virtual EducationDepartment? EducationDepartment { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual Store? Organization { get; set; }
    }
}
