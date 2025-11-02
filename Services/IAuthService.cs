using ExchangeSystem.Models;

namespace ExchangeSystem.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string username, string password);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> ValidateTokenAsync(string token);
        Task<User?> GetUserFromTokenAsync(string token);
    }
}



