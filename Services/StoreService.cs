using Microsoft.EntityFrameworkCore;
using ExchangeSystem.Data;
using ExchangeSystem.Models;

namespace ExchangeSystem.Services
{
    public class StoreService : IStoreService
    {
        private readonly ExchangeDbContext _context;

        public StoreService(ExchangeDbContext context)
        {
            _context = context;
        }

        public async Task<List<Store>> GetAllStoresAsync()
        {
            return await _context.Stores
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Store?> GetStoreByIdAsync(int id)
        {
            return await _context.Stores
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
        }

        public async Task<Store> CreateStoreAsync(Store store)
        {
            store.CreatedAt = DateTime.UtcNow;
            store.UpdatedAt = DateTime.UtcNow;
            store.IsActive = true;

            _context.Stores.Add(store);
            await _context.SaveChangesAsync();
            return store;
        }

        public async Task<Store> UpdateStoreAsync(Store store)
        {
            store.UpdatedAt = DateTime.UtcNow;
            _context.Stores.Update(store);
            await _context.SaveChangesAsync();
            return store;
        }

        public async Task<bool> DeleteStoreAsync(int id)
        {
            var store = await _context.Stores.FindAsync(id);
            if (store == null)
                return false;

            store.IsActive = false;
            store.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Store>> SearchStoresAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllStoresAsync();

            return await _context.Stores
                .Where(s => s.IsActive && (
                    s.Name.Contains(searchTerm) ||
                    (s.Address != null && s.Address.Contains(searchTerm)) ||
                    (s.City != null && s.City.Contains(searchTerm)) ||
                    (s.Manager != null && s.Manager.Contains(searchTerm))
                ))
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<List<Store>> GetStoresByEducationDepartmentAsync(int educationDepartmentId)
        {
            return await _context.Stores
                .Where(s => s.IsActive && s.EducationDepartmentId == educationDepartmentId)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}



