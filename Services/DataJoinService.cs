using Microsoft.EntityFrameworkCore;
using ExchangeSystem.Data;
using ExchangeSystem.Models;

namespace ExchangeSystem.Services
{
    public class DataJoinService : IDataJoinService
    {
        private readonly ExchangeDbContext _context;
        private readonly ILogger<DataJoinService> _logger;

        public DataJoinService(ExchangeDbContext context, ILogger<DataJoinService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<JoinResult> JoinDataAsync()
        {
            var result = new JoinResult();
            
            try
            {
                // Get all transactions with their related data
                var transactions = await _context.Transactions
                    .Include(t => t.Product)
                    .Include(t => t.Store)
                    .ToListAsync();

                result.RecordsJoined = transactions.Count;
                result.Success = true;
                result.Message = $"Успешно объединено {result.RecordsJoined} записей";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при объединении данных");
                result.Success = false;
                result.Message = $"Ошибка при объединении данных: {ex.Message}";
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        public async Task<List<JoinedRecord>> GetJoinedRecordsAsync(int page = 1, int pageSize = 50)
        {
            var skip = (page - 1) * pageSize;
            
            var transactions = await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Store)
                .OrderByDescending(t => t.TransactionDate)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return transactions.Select(t => new JoinedRecord
            {
                Id = t.Id,
                ProductName = t.Product?.Name ?? "Неизвестный продукт",
                ProductCode = t.Product?.Code ?? "",
                ProductCategory = t.Product?.Category ?? "",
                StoreName = t.Store?.Name ?? "Неизвестный магазин",
                StoreAddress = t.Store?.Address ?? "",
                TransactionDate = t.TransactionDate,
                DocumentNumber = t.DocumentNumber ?? "",
                Quantity = t.Quantity,
                Price = t.Price,
                TotalAmount = t.TotalAmount,
                Supplier = t.Supplier ?? "",
                ExpiryDate = t.ExpiryDate,
                Notes = t.Notes ?? "",
                CustomKey = GenerateCustomKey(t)
            }).ToList();
        }

        public async Task<int> GetJoinedRecordsCountAsync()
        {
            return await _context.Transactions.CountAsync();
        }

        public async Task<JoinResult> RegenerateKeysAsync()
        {
            var result = new JoinResult();
            
            try
            {
                var transactions = await _context.Transactions.ToListAsync();
                result.KeysGenerated = transactions.Count;
                result.Success = true;
                result.Message = $"Сгенерировано {result.KeysGenerated} ключей";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при генерации ключей");
                result.Success = false;
                result.Message = $"Ошибка при генерации ключей: {ex.Message}";
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        public async Task<object> GetJoinedDataAsync(int pageSize = 10, int pageNumber = 1)
        {
            var records = await GetJoinedRecordsAsync(pageNumber, pageSize);
            var totalCount = await GetJoinedRecordsCountAsync();
            
            return new
            {
                records,
                totalCount,
                pageNumber,
                pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        private string GenerateCustomKey(Transaction transaction)
        {
            // Generate a custom key based on transaction data
            var keyComponents = new[]
            {
                transaction.ProductId.ToString(),
                transaction.StoreId.ToString(),
                transaction.TransactionDate.ToString("yyyyMMdd"),
                transaction.DocumentNumber ?? "",
                transaction.Quantity.ToString("F3"),
                transaction.Price.ToString("F2")
            };

            var keyString = string.Join("_", keyComponents);
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(keyString))[..12];
        }
    }
}
