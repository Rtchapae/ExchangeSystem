using ExchangeSystem.Models;

namespace ExchangeSystem.Services
{
    public interface IStoreService
    {
        Task<List<Store>> GetAllStoresAsync();
        Task<Store?> GetStoreByIdAsync(int id);
        Task<Store> CreateStoreAsync(Store store);
        Task<Store> UpdateStoreAsync(Store store);
        Task<bool> DeleteStoreAsync(int id);
        Task<List<Store>> SearchStoresAsync(string searchTerm);
    }
}



