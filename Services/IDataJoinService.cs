using ExchangeSystem.Models;

namespace ExchangeSystem.Services
{
    public interface IDataJoinService
    {
        Task<JoinResult> JoinDataAsync();
        Task<List<JoinedRecord>> GetJoinedRecordsAsync(int page = 1, int pageSize = 50);
        Task<int> GetJoinedRecordsCountAsync();
        Task<object> GetJoinedDataAsync(int pageSize = 10, int pageNumber = 1);
        Task<JoinResult> RegenerateKeysAsync();
    }

    public class JoinResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int RecordsJoined { get; set; }
        public int KeysGenerated { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class JoinedRecord
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string StoreAddress { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public string Supplier { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string CustomKey { get; set; } = string.Empty;
    }
}
