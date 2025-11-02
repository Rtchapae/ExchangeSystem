using ExchangeSystem.Models;

namespace ExchangeSystem.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetAllTransactionsAsync();
        Task<Transaction?> GetTransactionByIdAsync(int id);
        Task<Transaction> CreateTransactionAsync(Transaction transaction);
        Task<Transaction> UpdateTransactionAsync(Transaction transaction);
        Task<bool> DeleteTransactionAsync(int id);
        Task<List<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Transaction>> GetTransactionsByProductAsync(int productId);
        Task<List<Transaction>> GetTransactionsByStoreAsync(int storeId);
    }
}



