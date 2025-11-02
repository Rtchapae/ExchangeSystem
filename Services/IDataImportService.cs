using ExchangeSystem.Models;

namespace ExchangeSystem.Services
{
    public interface IDataImportService
    {
        Task<ImportResult> ImportConsumptionDataAsync(Stream csvStream, string fileName, int? organizationId = null);
        Task<ImportResult> ImportProductDataAsync(Stream csvStream, string fileName);
        Task<ImportResult> ImportReceiptDataAsync(Stream csvStream, string fileName, int? organizationId = null);
        Task<ImportResult> ImportJsonDataAsync(string jsonContent, string fileName, int? educationDepartmentId = null, int? organizationId = null);
        Task<List<DataImportLog>> GetImportHistoryAsync();
        Task<DataImportLog?> GetImportLogByIdAsync(int id);
        Task<List<DataImportError>> GetImportErrorsAsync(int importLogId);
    }

    public class ImportResult
    {
        public bool IsSuccess { get; set; }
        public int ImportLogId { get; set; }
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
        public int SuccessRecords { get; set; }
        public int ErrorRecords { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string Message { get; set; } = string.Empty;
    }
}