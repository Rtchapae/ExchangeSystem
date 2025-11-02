using Microsoft.AspNetCore.Mvc;
using ExchangeSystem.Data;
using ExchangeSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ExchangeSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly ExchangeDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ExchangeDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("api/login")]
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

                // Проверяем пароль с помощью BCrypt или простой проверки для демо
                bool passwordValid = false;
                
                // Сначала пробуем BCrypt
                try
                {
                    passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
                }
                catch
                {
                    // Если BCrypt не работает, пробуем простую проверку
                    passwordValid = user.PasswordHash == request.Password;
                }
                
                if (!passwordValid)
                {
                    return BadRequest(new { success = false, message = "Неверное имя пользователя или пароль" });
                }

                // Обновляем время последнего входа
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Создаем claims для cookie авторизации
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Role, user.Role ?? "User"),
                    new Claim("FullName", user.FullName ?? user.Username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                    AllowRefresh = true
                };

                // Устанавливаем cookie авторизации
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

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

        [HttpPost("api/logout")]
        public async Task<IActionResult> Logout()
        {
            // Выходим из cookie авторизации
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return Ok(new { success = true, message = "Выход выполнен успешно" });
        }

        // MVC методы для веб-форм
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Имя пользователя и пароль обязательны";
                    return View();
                }

                // Ищем пользователя
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

                if (user == null)
                {
                    ViewBag.Error = "Неверное имя пользователя или пароль";
                    return View();
                }

                // Проверяем пароль
                bool passwordValid = false;
                
                try
                {
                    passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                }
                catch
                {
                    passwordValid = user.PasswordHash == password;
                }
                
                if (!passwordValid)
                {
                    ViewBag.Error = "Неверное имя пользователя или пароль";
                    return View();
                }

                // Обновляем время последнего входа
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Создаем claims для cookie авторизации
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim(ClaimTypes.Role, user.Role ?? "User"),
                    new Claim("FullName", user.FullName ?? user.Username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                    AllowRefresh = true
                };

                // Устанавливаем cookie авторизации
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("DataImport", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при входе пользователя {Username}", username);
                ViewBag.Error = "Внутренняя ошибка сервера";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> LogoutMvc()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
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