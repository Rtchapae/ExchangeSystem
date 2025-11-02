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
    public class OrganizationProductsController : ControllerBase
    {
        private readonly ExchangeDbContext _context;
        private readonly ILogger<OrganizationProductsController> _logger;

        public OrganizationProductsController(ExchangeDbContext context, ILogger<OrganizationProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/organizationproducts?organizationId=1
        [HttpGet]
        public async Task<IActionResult> GetOrganizationProducts([FromQuery] int organizationId)
        {
            try
            {
                var orgProducts = await _context.OrganizationProducts
                    .Where(op => op.OrganizationId == organizationId)
                    .Include(op => op.Product)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = orgProducts.Select(op => new
                    {
                        id = op.Id,
                        organizationId = op.OrganizationId,
                        productId = op.ProductId,
                        svsCode = op.SvsCode,
                        localPrice = op.LocalPrice,
                        isActive = op.IsActive,
                        product = new
                        {
                            id = op.Product.Id,
                            name = op.Product.Name,
                            code = op.Product.Code,
                            externalId = op.Product.ExternalId,
                            category = op.Product.Category,
                            unit = op.Product.Unit
                        }
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке продуктов организации {OrganizationId}", organizationId);
                return StatusCode(500, new { success = false, message = "Ошибка загрузки данных" });
            }
        }

        // POST: api/organizationproducts
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate([FromBody] OrganizationProductDto dto)
        {
            try
            {
                var existing = await _context.OrganizationProducts
                    .FirstOrDefaultAsync(op => op.OrganizationId == dto.OrganizationId && op.ProductId == dto.ProductId);

                if (existing != null)
                {
                    // Обновляем
                    existing.SvsCode = dto.SvsCode;
                    existing.LocalPrice = dto.LocalPrice;
                    existing.IsActive = dto.IsActive;
                    existing.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Создаем новый
                    var orgProduct = new OrganizationProduct
                    {
                        OrganizationId = dto.OrganizationId,
                        ProductId = dto.ProductId,
                        SvsCode = dto.SvsCode,
                        LocalPrice = dto.LocalPrice,
                        IsActive = dto.IsActive,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.OrganizationProducts.Add(orgProduct);
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Код СВС сохранен" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении кода СВС");
                return StatusCode(500, new { success = false, message = "Ошибка сохранения" });
            }
        }
    }

    public class OrganizationProductDto
    {
        public int OrganizationId { get; set; }
        public int ProductId { get; set; }
        public string? SvsCode { get; set; }
        public decimal? LocalPrice { get; set; }
        public bool IsActive { get; set; } = true;
    }
}


