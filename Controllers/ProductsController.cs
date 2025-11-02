using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExchangeSystem.Services;
using ExchangeSystem.Models;

namespace ExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] string? search = null)
        {
            try
            {
                var products = string.IsNullOrEmpty(search) 
                    ? await _productService.GetAllProductsAsync()
                    : await _productService.SearchProductsAsync(search);

                return Ok(new
                {
                    success = true,
                    data = products
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении продуктов");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { success = false, message = "Продукт не найден" });
                }

                return Ok(new
                {
                    success = true,
                    data = product
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении продукта {ProductId}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Неверные данные", errors = ModelState });
                }

                var createdProduct = await _productService.CreateProductAsync(product);
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, new
                {
                    success = true,
                    message = "Продукт создан",
                    data = createdProduct
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании продукта");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            try
            {
                if (id != product.Id)
                {
                    return BadRequest(new { success = false, message = "Неверный ID продукта" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Неверные данные", errors = ModelState });
                }

                var updatedProduct = await _productService.UpdateProductAsync(product);
                return Ok(new
                {
                    success = true,
                    message = "Продукт обновлен",
                    data = updatedProduct
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении продукта {ProductId}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Продукт не найден" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Продукт удален"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении продукта {ProductId}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }
    }
}


