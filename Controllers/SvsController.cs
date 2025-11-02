using Microsoft.AspNetCore.Mvc;
using ExchangeSystem.Models;
using ExchangeSystem.Services;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace ExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SvsController : ControllerBase
    {
        private readonly IProductMappingService _mappingService;
        private readonly ILogger<SvsController> _logger;

        public SvsController(IProductMappingService mappingService, ILogger<SvsController> logger)
        {
            _mappingService = mappingService;
            _logger = logger;
        }

        /// <summary>
        /// Получение справочника СВС и автоматическое сопоставление с продуктами
        /// </summary>
        [HttpPost("materials")]
        public async Task<IActionResult> ImportSvsMaterials([FromBody] SvsMaterialsRequest request, [FromQuery] int? educationDepartmentId = null)
        {
            try
            {
                if (request?.MatItem == null || !request.MatItem.Any())
                {
                    return BadRequest(new { success = false, message = "Справочник СВС пуст" });
                }

                _logger.LogInformation($"Получен справочник СВС: {request.MatItem.Count} материалов");

                // Выполняем автоматическое сопоставление
                var mappingResults = await _mappingService.MapProductsToSvsAsync(request.MatItem, educationDepartmentId);

                var mappedCount = mappingResults.Count(m => !string.IsNullOrEmpty(m.SvsCode));
                var unmappedCount = mappingResults.Count - mappedCount;

                return Ok(new
                {
                    success = true,
                    message = "Справочник СВС обработан",
                    totalProducts = mappingResults.Count,
                    autoMapped = mappedCount,
                    unmapped = unmappedCount,
                    educationDepartmentId = educationDepartmentId,
                    mappings = mappingResults
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке справочника СВС");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Получение результатов сопоставления для организации
        /// </summary>
        [HttpGet("mappings")]
        public async Task<IActionResult> GetMappings([FromQuery] int? organizationId = null)
        {
            try
            {
                var mappings = await _mappingService.GetMappingResultsAsync(organizationId);
                
                return Ok(new
                {
                    success = true,
                    data = mappings,
                    totalCount = mappings.Count,
                    mappedCount = mappings.Count(m => !string.IsNullOrEmpty(m.SvsCode)),
                    unmappedCount = mappings.Count(m => string.IsNullOrEmpty(m.SvsCode))
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении сопоставлений");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Сохранение сопоставления продукта с СВС
        /// </summary>
        [HttpPost("mappings")]
        public async Task<IActionResult> SaveMapping([FromBody] SaveMappingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Неверные данные" });
                }

                var success = await _mappingService.SaveMappingAsync(
                    request.ProductId, 
                    request.SvsMatId, 
                    request.SvsCode, 
                    request.OrganizationId);

                if (success)
                {
                    return Ok(new { success = true, message = "Сопоставление сохранено" });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Ошибка при сохранении" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении сопоставления");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Экспорт сопоставлений в JSON
        /// </summary>
        [HttpGet("export")]
        public async Task<IActionResult> ExportMappings([FromQuery] int? organizationId = null)
        {
            try
            {
                var jsonData = await _mappingService.ExportMappingsToJsonAsync(organizationId);
                
                var fileName = organizationId.HasValue 
                    ? $"svs_mappings_org_{organizationId}_{DateTime.Now:yyyyMMdd}.json"
                    : $"svs_mappings_global_{DateTime.Now:yyyyMMdd}.json";

                return File(
                    System.Text.Encoding.UTF8.GetBytes(jsonData),
                    "application/json",
                    fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при экспорте сопоставлений");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Получение статистики сопоставлений
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetMappingStats([FromQuery] int? organizationId = null)
        {
            try
            {
                var mappings = await _mappingService.GetMappingResultsAsync(organizationId);
                
                var stats = new
                {
                    totalProducts = mappings.Count,
                    mappedProducts = mappings.Count(m => !string.IsNullOrEmpty(m.SvsCode)),
                    unmappedProducts = mappings.Count(m => string.IsNullOrEmpty(m.SvsCode)),
                    autoMappedProducts = mappings.Count(m => m.IsAutoMapped),
                    manualMappedProducts = mappings.Count(m => !m.IsAutoMapped && !string.IsNullOrEmpty(m.SvsCode)),
                    averageConfidence = mappings.Where(m => m.IsAutoMapped).Average(m => m.Confidence),
                    organizationId = organizationId
                };

                return Ok(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статистики");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Получение статистики справочника СВС
        /// </summary>
        [HttpGet("svs-stats")]
        public async Task<IActionResult> GetSvsStats()
        {
            try
            {
                var stats = await _mappingService.GetSvsStatsAsync();
                return Ok(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статистики СВС");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Получение истории обновлений справочника СВС
        /// </summary>
        [HttpGet("updates")]
        public async Task<IActionResult> GetUpdates()
        {
            try
            {
                var updates = await _mappingService.GetSvsUpdatesAsync();
                return Ok(new { success = true, data = updates });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении обновлений СВС");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }
    }

    public class SaveMappingRequest
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public int SvsMatId { get; set; }
        
        [Required]
        public string SvsCode { get; set; } = string.Empty;
        
        public int? OrganizationId { get; set; }
        
        public string? Notes { get; set; }
    }

    public class SvsMaterialsRequest
    {
        [Required]
        public List<SvsMaterialItem> MatItem { get; set; } = new List<SvsMaterialItem>();
        
        public string? Notes { get; set; }
        
        public string? UpdateSource { get; set; } = "Manual";
    }
}
