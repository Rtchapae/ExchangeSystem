using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExchangeSystem.Data;
using ExchangeSystem.Models;

namespace ExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportsController : ControllerBase
    {
        private readonly ExchangeDbContext _context;
        private readonly ILogger<ExportsController> _logger;

        public ExportsController(ExchangeDbContext context, ILogger<ExportsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("consumption")]
        public async Task<IActionResult> ExportConsumption(
            [FromQuery] int? organizationId = null,
            [FromQuery] int? educationDepartmentId = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            try
            {
                var query = _context.ProductConsumptions
                    .Include(pc => pc.Product)
                    .AsQueryable();

                if (from.HasValue)
                {
                    var f = DateTime.SpecifyKind(from.Value, DateTimeKind.Utc);
                    query = query.Where(x => x.ConsumptionDate >= f);
                }
                if (to.HasValue)
                {
                    var t = DateTime.SpecifyKind(to.Value, DateTimeKind.Utc);
                    query = query.Where(x => x.ConsumptionDate <= t);
                }
                if (organizationId.HasValue)
                {
                    query = query.Where(x => x.OrganizationId == organizationId.Value);
                }
                if (educationDepartmentId.HasValue)
                {
                    query = query.Where(x => x.EducationDepartmentId == educationDepartmentId.Value);
                }

                var items = await query
                    .OrderBy(x => x.ConsumptionDate)
                    .ToListAsync();

                // Получаем сопоставления СВС для организации при наличии
                Dictionary<int, string?> productIdToSvs = new();
                if (organizationId.HasValue)
                {
                    productIdToSvs = await _context.OrganizationProducts
                        .Where(op => op.OrganizationId == organizationId.Value)
                        .ToDictionaryAsync(op => op.ProductId, op => op.SvsCode);
                }

                var export = new
                {
                    ExportType = "consumption",
                    OrganizationId = organizationId,
                    EducationDepartmentId = educationDepartmentId,
                    From = from,
                    To = to,
                    GeneratedAt = DateTime.UtcNow,
                    Items = items.Select(i => new
                    {
                        Date = i.ConsumptionDate,
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Unit = i.Unit,
                        Quantity = i.Quantity,
                        Price = i.Price,
                        TotalCost = i.TotalCost,
                        CategoryId = i.CategoryId,
                        CategoryName = i.CategoryName,
                        SvsCode = (organizationId.HasValue && productIdToSvs.TryGetValue(i.ProductId, out var code) ? code : i.Product.SvsCode)
                    })
                };

                var json = System.Text.Json.JsonSerializer.Serialize(export, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                var fileName = BuildFileName("consumption", organizationId, educationDepartmentId, from, to);
                return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка экспорта расхода");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("receipts")]
        public async Task<IActionResult> ExportReceipts(
            [FromQuery] int? organizationId = null,
            [FromQuery] int? educationDepartmentId = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            try
            {
                var query = _context.ProductReceipts
                    .Include(pr => pr.Product)
                    .AsQueryable();

                if (from.HasValue)
                {
                    var f = DateTime.SpecifyKind(from.Value, DateTimeKind.Utc);
                    query = query.Where(x => x.ReceiptDate >= f);
                }
                if (to.HasValue)
                {
                    var t = DateTime.SpecifyKind(to.Value, DateTimeKind.Utc);
                    query = query.Where(x => x.ReceiptDate <= t);
                }
                if (organizationId.HasValue)
                {
                    query = query.Where(x => x.OrganizationId == organizationId.Value);
                }
                if (educationDepartmentId.HasValue)
                {
                    query = query.Where(x => x.EducationDepartmentId == educationDepartmentId.Value);
                }

                var items = await query
                    .OrderBy(x => x.ReceiptDate)
                    .ToListAsync();

                Dictionary<int, string?> productIdToSvs = new();
                if (organizationId.HasValue)
                {
                    productIdToSvs = await _context.OrganizationProducts
                        .Where(op => op.OrganizationId == organizationId.Value)
                        .ToDictionaryAsync(op => op.ProductId, op => op.SvsCode);
                }

                var export = new
                {
                    ExportType = "receipts",
                    OrganizationId = organizationId,
                    EducationDepartmentId = educationDepartmentId,
                    From = from,
                    To = to,
                    GeneratedAt = DateTime.UtcNow,
                    Items = items.Select(i => new
                    {
                        Date = i.ReceiptDate,
                        DocumentNumber = i.DocumentNumber,
                        SupplierName = i.SupplierName,
                        SupplierUnp = i.SupplierUnp,
                        ContractDate = i.ContractDate,
                        ContractNumber = i.ContractNumber,
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Unit = i.Unit,
                        Quantity = i.Quantity,
                        Price = i.Price,
                        TotalCost = i.TotalCost,
                        SvsCode = (organizationId.HasValue && productIdToSvs.TryGetValue(i.ProductId, out var code) ? code : i.Product.SvsCode)
                    })
                };

                var json = System.Text.Json.JsonSerializer.Serialize(export, new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                var fileName = BuildFileName("receipts", organizationId, educationDepartmentId, from, to);
                return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка экспорта приходов");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        private static string BuildFileName(string type, int? orgId, int? uoId, DateTime? from, DateTime? to)
        {
            var scope = orgId.HasValue ? $"org_{orgId}" : (uoId.HasValue ? $"uo_{uoId}" : "all");
            var range = (from.HasValue || to.HasValue)
                ? $"_{from:yyyyMMdd}-{to:yyyyMMdd}"
                : "";
            return $"export_{type}_{scope}{range}_{DateTime.UtcNow:yyyyMMddHHmmss}.json";
        }
    }
}


