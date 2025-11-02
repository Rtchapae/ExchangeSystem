using Microsoft.AspNetCore.Mvc;

namespace ExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "Test API is working!", timestamp = DateTime.Now });
        }

        [HttpPost("upload")]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            return Ok(new { 
                message = "File uploaded successfully", 
                fileName = file.FileName, 
                size = file.Length 
            });
        }
    }
}

