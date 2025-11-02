using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace ExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("password")]
        public IActionResult TestPassword()
        {
            string storedHash = "$2a$11$SWL1sbN2PR60DND5RiuNbePvjAVv9yMo8kmSpFZzMdUvEZMEDjJZC";
            string[] passwords = { "admin123", "admin", "password", "123456" };
            
            var results = new List<object>();
            
            foreach (string pwd in passwords)
            {
                bool verified = BCrypt.Net.BCrypt.Verify(pwd, storedHash);
                results.Add(new { password = pwd, verified = verified });
            }
            
            return Ok(new { storedHash, results });
        }
    }
}