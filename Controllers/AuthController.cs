using Microsoft.AspNetCore.Mvc;
using ExchangeSystem.Data;
using ExchangeSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ExchangeDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ExchangeDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { success = false, message = "Имя пользователя и пароль обязательны" });
                }

                // Ищем пользователя
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

                if (user == null)
                {
                    return BadRequest(new { success = false, message = "Неверное имя пользователя или пароль" });
                }

                // Проверяем пароль (простая проверка для демо)
                if (user.PasswordHash != request.Password && user.PasswordHash != HashPassword(request.Password))
                {
                    return BadRequest(new { success = false, message = "Неверное имя пользователя или пароль" });
                }

                // Обновляем время последнего входа
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Генерируем простой токен (в реальном приложении используйте JWT)
                var token = GenerateSimpleToken(user.Id, user.Username);

                return Ok(new
                {
                    success = true,
                    token = token,
                    username = user.Username,
                    message = "Вход выполнен успешно"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при входе пользователя {Username}", request.Username);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { success = true, message = "Выход выполнен успешно" });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "salt"));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private string GenerateSimpleToken(int userId, string username)
        {
            var tokenData = $"{userId}:{username}:{DateTime.UtcNow.Ticks}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenData));
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}