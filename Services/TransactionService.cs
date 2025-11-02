using Microsoft.EntityFrameworkCore;
using ExchangeSystem.Data;
using ExchangeSystem.Models;

namespace ExchangeSystem.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ExchangeDbContext _context;

        public TransactionService(ExchangeDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Store)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Store)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            transaction.CreatedAt = DateTime.UtcNow;
            transaction.UpdatedAt = DateTime.UtcNow;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> UpdateTransactionAsync(Transaction transaction)
        {
            transaction.UpdatedAt = DateTime.UtcNow;
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Store)
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByProductAsync(int productId)
        {
            return await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Store)
                .Where(t => t.ProductId == productId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByStoreAsync(int storeId)
        {
            return await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Store)
                .Where(t => t.StoreId == storeId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
    }
}



