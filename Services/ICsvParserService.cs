using ExchangeSystem.Models;

namespace ExchangeSystem.Services
{
    public interface ICsvParserService
    {
        Task<CsvParseResult> ParseConsumptionDataAsync(Stream csvStream, string fileName);
        Task<CsvParseResult> ParseProductDataAsync(Stream csvStream, string fileName);
        Task<CsvParseResult> ParseReceiptDataAsync(Stream csvStream, string fileName);
    }

    public class CsvParseResult
    {
        public bool IsSuccess { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<object> ParsedData { get; set; } = new List<object>();
        public int TotalRows { get; set; }
        public int ProcessedRows { get; set; }
        public int ErrorRows { get; set; }
    }

    public class ConsumptionDataRow
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public DateTime ConsumptionDate { get; set; }
        public decimal NurseryQuantity { get; set; }
        public decimal NurseryCost { get; set; }
        public decimal KindergartenQuantity { get; set; }
        public decimal KindergartenCost { get; set; }
        public decimal StaffQuantity { get; set; }
        public decimal StaffCost { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class ReceiptDataRow
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public DateTime ReceiptDate { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string? SupplierUnp { get; set; }
        public DateTime? ContractDate { get; set; }
        public string? ContractNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalCost { get; set; }
    }

    public class ProductDataRow
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
