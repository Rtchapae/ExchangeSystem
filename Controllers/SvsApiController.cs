using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ExchangeSystem.Data;
using ExchangeSystem.Models;

namespace ExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/svs")]
    [Authorize]
    public class SvsApiController : ControllerBase
    {
        private readonly ExchangeDbContext _context;
        private readonly ILogger<SvsApiController> _logger;

        public SvsApiController(
            ExchangeDbContext context,
            ILogger<SvsApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Получение данных потребления по организации и дате
        /// </summary>
        [HttpGet("consumption")]
        public async Task<IActionResult> GetConsumptionData(
            [FromQuery] string organizationId,
            [FromQuery] DateTime date)
        {
            try
            {
                if (string.IsNullOrEmpty(organizationId))
                {
                    return BadRequest("OrganizationId обязателен");
                }

                var consumptionData = await _context.ProductConsumptions
                    .Include(pc => pc.Product)
                    .Include(pc => pc.Category)
                    .Where(pc => pc.ConsumptionDate.Date == date.Date)
                    .ToListAsync();

                var result = consumptionData.Select(pc => new
                {
                    Date = pc.ConsumptionDate.ToString("yyyy-MM-dd"),
                    ProductId = pc.ProductId,
                    ProductName = pc.ProductName,
                    Unit = pc.Unit,
                    Quantity = pc.Quantity,
                    Price = pc.Price,
                    TotalCost = pc.TotalCost,
                    CategoryId = pc.CategoryId,
                    CategoryName = pc.CategoryName,
                    NurseryQuantity = pc.NurseryQuantity,
                    NurseryCost = pc.NurseryCost,
                    KindergartenQuantity = pc.KindergartenQuantity,
                    KindergartenCost = pc.KindergartenCost,
                    StaffQuantity = pc.StaffQuantity,
                    StaffCost = pc.StaffCost
                }).ToList();

                return Ok(new
                {
                    success = true,
                    organizationId = organizationId,
                    date = date.ToString("yyyy-MM-dd"),
                    data = result,
                    count = result.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении данных потребления для организации {OrganizationId} на дату {Date}", 
                    organizationId, date);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Получение данных прихода по организации и дате
        /// </summary>
        [HttpGet("receipts")]
        public async Task<IActionResult> GetReceiptData(
            [FromQuery] string organizationId,
            [FromQuery] DateTime date)
        {
            try
            {
                if (string.IsNullOrEmpty(organizationId))
                {
                    return BadRequest("OrganizationId обязателен");
                }

                var receiptData = await _context.ProductReceipts
                    .Include(pr => pr.Product)
                    .Where(pr => pr.ReceiptDate.Date == date.Date)
                    .ToListAsync();

                var result = receiptData.Select(pr => new
                {
                    ReceiptDate = pr.ReceiptDate.ToString("yyyy-MM-dd"),
                    DocumentNumber = pr.DocumentNumber,
                    SupplierName = pr.SupplierName,
                    SupplierUnp = pr.SupplierUnp,
                    ContractDate = pr.ContractDate?.ToString("yyyy-MM-dd"),
                    ContractNumber = pr.ContractNumber,
                    ProductId = pr.ProductId,
                    ProductName = pr.ProductName,
                    Unit = pr.Unit,
                    Quantity = pr.Quantity,
                    Price = pr.Price,
                    TotalCost = pr.TotalCost
                }).ToList();

                return Ok(new
                {
                    success = true,
                    organizationId = organizationId,
                    date = date.ToString("yyyy-MM-dd"),
                    data = result,
                    count = result.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении данных прихода для организации {OrganizationId} на дату {Date}", 
                    organizationId, date);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Получение справочника продуктов
        /// </summary>
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => p.IsActive)
                    .Select(p => new
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Code = p.Code,
                        Unit = p.Unit,
                        Category = p.Category,
                        Price = p.Price
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = products,
                    count = products.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении справочника продуктов");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Получение сопоставлений продуктов
        /// </summary>
        [HttpGet("product-mappings")]
        public async Task<IActionResult> GetProductMappings()
        {
            try
            {
                var mappings = await _context.ProductMappings
                    .Include(pm => pm.SvsProduct)
                    .Where(pm => pm.IsApproved)
                    .Select(pm => new
                    {
                        Id = pm.Id,
                        ExternalProductCode = pm.ExternalProductCode,
                        ExternalProductName = pm.ExternalProductName,
                        ExternalUnit = pm.ExternalUnit,
                        SvsProductId = pm.SvsProductId,
                        SvsProductName = pm.SvsProductName,
                        SvsUnit = pm.SvsUnit
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = mappings,
                    count = mappings.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении сопоставлений продуктов");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Получение данных по всем организациям на дату
        /// </summary>
        [HttpGet("organizations-data")]
        public async Task<IActionResult> GetOrganizationsData([FromQuery] DateTime date)
        {
            try
            {
                var consumptionData = await _context.ProductConsumptions
                    .Include(pc => pc.Product)
                    .Include(pc => pc.Category)
                    .Where(pc => pc.ConsumptionDate.Date == date.Date)
                    .GroupBy(pc => pc.CategoryId)
                    .Select(g => new
                    {
                        CategoryId = g.Key,
                        CategoryName = g.First().CategoryName,
                        TotalQuantity = g.Sum(pc => pc.Quantity),
                        TotalCost = g.Sum(pc => pc.TotalCost),
                        Products = g.Select(pc => new
                        {
                            ProductId = pc.ProductId,
                            ProductName = pc.ProductName,
                            Unit = pc.Unit,
                            Quantity = pc.Quantity,
                            Price = pc.Price,
                            TotalCost = pc.TotalCost
                        }).ToList()
                    })
                    .ToListAsync();

                var receiptData = await _context.ProductReceipts
                    .Include(pr => pr.Product)
                    .Where(pr => pr.ReceiptDate.Date == date.Date)
                    .Select(pr => new
                    {
                        ReceiptDate = pr.ReceiptDate.ToString("yyyy-MM-dd"),
                        DocumentNumber = pr.DocumentNumber,
                        SupplierName = pr.SupplierName,
                        ProductId = pr.ProductId,
                        ProductName = pr.ProductName,
                        Unit = pr.Unit,
                        Quantity = pr.Quantity,
                        Price = pr.Price,
                        TotalCost = pr.TotalCost
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    date = date.ToString("yyyy-MM-dd"),
                    consumption = consumptionData,
                    receipts = receiptData,
                    consumptionCount = consumptionData.Count,
                    receiptsCount = receiptData.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении данных по организациям на дату {Date}", date);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Проверка доступности данных на дату
        /// </summary>
        [HttpGet("check-data-availability")]
        public async Task<IActionResult> CheckDataAvailability([FromQuery] DateTime date)
        {
            try
            {
                var hasConsumptionData = await _context.ProductConsumptions
                    .AnyAsync(pc => pc.ConsumptionDate.Date == date.Date);

                var hasReceiptData = await _context.ProductReceipts
                    .AnyAsync(pr => pr.ReceiptDate.Date == date.Date);

                var consumptionCount = await _context.ProductConsumptions
                    .CountAsync(pc => pc.ConsumptionDate.Date == date.Date);

                var receiptCount = await _context.ProductReceipts
                    .CountAsync(pr => pr.ReceiptDate.Date == date.Date);

                return Ok(new
                {
                    success = true,
                    date = date.ToString("yyyy-MM-dd"),
                    hasData = hasConsumptionData || hasReceiptData,
                    hasConsumptionData = hasConsumptionData,
                    hasReceiptData = hasReceiptData,
                    consumptionCount = consumptionCount,
                    receiptCount = receiptCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке доступности данных на дату {Date}", date);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }
    }
}

