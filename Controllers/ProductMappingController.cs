using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ExchangeSystem.Data;
using ExchangeSystem.Models;

namespace ExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductMappingController : ControllerBase
    {
        private readonly ExchangeDbContext _context;
        private readonly ILogger<ProductMappingController> _logger;

        public ProductMappingController(
            ExchangeDbContext context,
            ILogger<ProductMappingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMappings()
        {
            try
            {
                var mappings = await _context.ProductMappings
                    .Include(pm => pm.SvsProduct)
                    .OrderBy(pm => pm.ExternalProductName)
                    .ToListAsync();

                return Ok(mappings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении сопоставлений продуктов");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMapping(int id)
        {
            try
            {
                var mapping = await _context.ProductMappings
                    .Include(pm => pm.SvsProduct)
                    .FirstOrDefaultAsync(pm => pm.Id == id);

                if (mapping == null)
                {
                    return NotFound("Сопоставление не найдено");
                }

                return Ok(mapping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении сопоставления {Id}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMapping([FromBody] ProductMapping mapping)
        {
            try
            {
                if (string.IsNullOrEmpty(mapping.ExternalProductCode) || string.IsNullOrEmpty(mapping.ExternalProductName))
                {
                    return BadRequest("Код и название внешнего продукта обязательны");
                }

                // Проверка на дубликаты
                var existingMapping = await _context.ProductMappings
                    .FirstOrDefaultAsync(pm => pm.ExternalProductCode == mapping.ExternalProductCode);

                if (existingMapping != null)
                {
                    return BadRequest("Сопоставление с таким кодом продукта уже существует");
                }

                mapping.CreatedAt = DateTime.UtcNow;
                mapping.UpdatedAt = DateTime.UtcNow;

                _context.ProductMappings.Add(mapping);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetMapping), new { id = mapping.Id }, mapping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании сопоставления");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMapping(int id, [FromBody] ProductMapping mapping)
        {
            try
            {
                var existingMapping = await _context.ProductMappings.FindAsync(id);
                if (existingMapping == null)
                {
                    return NotFound("Сопоставление не найдено");
                }

                existingMapping.ExternalProductName = mapping.ExternalProductName;
                existingMapping.ExternalUnit = mapping.ExternalUnit;
                existingMapping.ExternalGroup = mapping.ExternalGroup;
                existingMapping.ContractNumber = mapping.ContractNumber;
                existingMapping.SvsProductId = mapping.SvsProductId;
                existingMapping.SvsProductName = mapping.SvsProductName;
                existingMapping.SvsUnit = mapping.SvsUnit;
                existingMapping.SvsGroup = mapping.SvsGroup;
                existingMapping.IsApproved = mapping.IsApproved;
                existingMapping.Notes = mapping.Notes;
                existingMapping.UpdatedAt = DateTime.UtcNow;

                if (mapping.IsApproved && !existingMapping.IsApproved)
                {
                    existingMapping.ApprovedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return Ok(existingMapping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении сопоставления {Id}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMapping(int id)
        {
            try
            {
                var mapping = await _context.ProductMappings.FindAsync(id);
                if (mapping == null)
                {
                    return NotFound("Сопоставление не найдено");
                }

                _context.ProductMappings.Remove(mapping);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении сопоставления {Id}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost("auto-map")]
        public async Task<IActionResult> AutoMapProducts()
        {
            try
            {
                var unmappedProducts = await _context.ProductMappings
                    .Where(pm => pm.SvsProductId == null)
                    .ToListAsync();

                var svsProducts = await _context.Products.ToListAsync();
                var mappedCount = 0;

                foreach (var mapping in unmappedProducts)
                {
                    // Поиск по коду
                    var svsProduct = svsProducts.FirstOrDefault(p => 
                        p.Code == mapping.ExternalProductCode);

                    // Поиск по названию (частичное совпадение)
                    if (svsProduct == null)
                    {
                        svsProduct = svsProducts.FirstOrDefault(p => 
                            p.Name.Contains(mapping.ExternalProductName) ||
                            mapping.ExternalProductName.Contains(p.Name));
                    }

                    if (svsProduct != null)
                    {
                        mapping.SvsProductId = svsProduct.Id;
                        mapping.SvsProductName = svsProduct.Name;
                        mapping.SvsUnit = svsProduct.Unit;
                        mapping.SvsGroup = svsProduct.Category;
                        mapping.IsAutoMapped = true;
                        mapping.UpdatedAt = DateTime.UtcNow;
                        mappedCount++;
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = $"Автоматическое сопоставление завершено. Сопоставлено {mappedCount} из {unmappedProducts.Count} продуктов.",
                    mappedCount = mappedCount,
                    totalCount = unmappedProducts.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при автоматическом сопоставлении");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("unmapped")]
        public async Task<IActionResult> GetUnmappedProducts()
        {
            try
            {
                var unmappedProducts = await _context.ProductMappings
                    .Where(pm => pm.SvsProductId == null)
                    .OrderBy(pm => pm.ExternalProductName)
                    .ToListAsync();

                return Ok(unmappedProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении несопоставленных продуктов");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("search-svs-products")]
        public async Task<IActionResult> SearchSvsProducts([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    return Ok(new List<Product>());
                }

                var products = await _context.Products
                    .Where(p => p.Name.Contains(query) || 
                               (p.Code != null && p.Code.Contains(query)))
                    .Take(20)
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске продуктов СВС");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }
    }
}

