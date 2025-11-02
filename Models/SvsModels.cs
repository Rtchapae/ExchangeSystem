using System.ComponentModel.DataAnnotations;

namespace ExchangeSystem.Models
{
    /// <summary>
    /// Модель для получения справочника СВС
    /// </summary>
    public class SvsMaterialItem
    {
        public int GroupMatId { get; set; }
        public int MatId { get; set; }
        public int MeasureId { get; set; }
        public string NameGroupMat { get; set; } = string.Empty;
        public string NameMat { get; set; } = string.Empty;
        public string NameMeasure { get; set; } = string.Empty;
    }

    public class SvsMaterialsRequest
    {
        public List<SvsMaterialItem> MatItem { get; set; } = new List<SvsMaterialItem>();
    }

    /// <summary>
    /// Результат сопоставления продукта с СВС
    /// </summary>
    public class ProductMappingResult
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string? SvsCode { get; set; }
        public string? SvsName { get; set; }
        public int? SvsMatId { get; set; }
        public int? SvsGroupId { get; set; }
        public string? SvsGroupName { get; set; }
        public string? SvsMeasure { get; set; }
        public bool IsAutoMapped { get; set; }
        public double Confidence { get; set; } // Уверенность в сопоставлении (0-1)
        public string? MappingNotes { get; set; }
    }

    /// <summary>
    /// Модель для импорта DBF файлов
    /// </summary>
    public class DbfDataRow
    {
        public DateTime DateTime { get; set; }
        public string Cipher { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public string EatingId { get; set; } = string.Empty;
        public string Eating { get; set; } = string.Empty;
        public string Measure { get; set; } = string.Empty;
        public decimal WeightTotal { get; set; }
        public decimal EatingCount { get; set; }
        public decimal PriceOneKg { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
