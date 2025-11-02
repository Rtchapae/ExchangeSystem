using Microsoft.AspNetCore.Mvc;
using ExchangeSystem.Services;
using ExchangeSystem.Models;

namespace ExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataImportController : ControllerBase
    {
        private readonly IDataImportService _dataImportService;
        private readonly IDbfImportService _dbfImportService;
        private readonly ILogger<DataImportController> _logger;

        public DataImportController(
            IDataImportService dataImportService,
            IDbfImportService dbfImportService,
            ILogger<DataImportController> logger)
        {
            _dataImportService = dataImportService;
            _dbfImportService = dbfImportService;
            _logger = logger;
        }

        [HttpPost("consumption")]
        public async Task<IActionResult> ImportConsumptionData(IFormFile file, [FromForm] int? organizationId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пуст");
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Поддерживаются только CSV файлы");
            }

            try
            {
                _logger.LogInformation("Импорт данных потребления для организации {OrganizationId}", organizationId ?? 0);
                
                using var stream = file.OpenReadStream();
                var result = await _dataImportService.ImportConsumptionDataAsync(stream, file.FileName, organizationId);

                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        organizationId = organizationId,
                        importLogId = result.ImportLogId,
                        totalRecords = result.TotalRecords,
                        processedRecords = result.ProcessedRecords,
                        successRecords = result.SuccessRecords,
                        errorRecords = result.ErrorRecords
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message,
                        errors = result.Errors,
                        importLogId = result.ImportLogId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте данных потребления");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost("products")]
        public async Task<IActionResult> ImportProductData(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пуст");
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Поддерживаются только CSV файлы");
            }

            try
            {
                using var stream = file.OpenReadStream();
                var result = await _dataImportService.ImportProductDataAsync(stream, file.FileName);

                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        importLogId = result.ImportLogId,
                        totalRecords = result.TotalRecords,
                        processedRecords = result.ProcessedRecords,
                        successRecords = result.SuccessRecords,
                        errorRecords = result.ErrorRecords
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message,
                        errors = result.Errors,
                        importLogId = result.ImportLogId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте данных продуктов");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost("receipts")]
        public async Task<IActionResult> ImportReceiptData(IFormFile file, [FromForm] int? organizationId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пуст");
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Поддерживаются только CSV файлы");
            }

            try
            {
                _logger.LogInformation("Импорт данных прихода для организации {OrganizationId}", organizationId ?? 0);
                
                using var stream = file.OpenReadStream();
                var result = await _dataImportService.ImportReceiptDataAsync(stream, file.FileName, organizationId);

                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        organizationId = organizationId,
                        importLogId = result.ImportLogId,
                        totalRecords = result.TotalRecords,
                        processedRecords = result.ProcessedRecords,
                        successRecords = result.SuccessRecords,
                        errorRecords = result.ErrorRecords
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message,
                        errors = result.Errors,
                        importLogId = result.ImportLogId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте данных прихода");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetImportHistory()
        {
            try
            {
                var history = await _dataImportService.GetImportHistoryAsync();
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении истории импорта");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("history/{id}")]
        public async Task<IActionResult> GetImportLog(int id)
        {
            try
            {
                var importLog = await _dataImportService.GetImportLogByIdAsync(id);
                if (importLog == null)
                {
                    return NotFound("Лог импорта не найден");
                }

                return Ok(importLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении лога импорта {Id}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("errors/{importLogId}")]
        public async Task<IActionResult> GetImportErrors(int importLogId)
        {
            try
            {
                var errors = await _dataImportService.GetImportErrorsAsync(importLogId);
                return Ok(errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении ошибок импорта {ImportLogId}", importLogId);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost("dbf")]
        public async Task<IActionResult> ImportDbfData(IFormFile file, [FromForm] int? organizationId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пуст");
            }

            try
            {
                using var stream = file.OpenReadStream();
                var result = await _dbfImportService.ImportDbfDataAsync(stream, file.FileName, organizationId);

                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Импорт DBF файла завершен",
                        importLogId = result.ImportLogId,
                        totalRecords = result.TotalRecords,
                        processedRecords = result.ProcessedRecords,
                        successRecords = result.SuccessRecords,
                        errorRecords = result.ErrorRecords,
                        organizationId = organizationId
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Ошибка импорта DBF файла",
                        errors = result.Errors
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте DBF файла {FileName}", file.FileName);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost("dbf/consumption")]
        public async Task<IActionResult> ImportDbfConsumption(IFormFile file, [FromForm] int? organizationId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пуст");
            }

            try
            {
                using var stream = file.OpenReadStream();
                var result = await _dbfImportService.ImportDbfDataAsync(stream, file.FileName, organizationId);
                if (result.IsSuccess)
                {
                    return Ok(new { success = true, message = "Импорт DBF расхода завершен", importLogId = result.ImportLogId, totalRecords = result.TotalRecords, processedRecords = result.ProcessedRecords, successRecords = result.SuccessRecords, errorRecords = result.ErrorRecords, organizationId });
                }
                return BadRequest(new { success = false, message = "Ошибка импорта DBF расхода", errors = result.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте DBF расхода {FileName}", file.FileName);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost("dbf/receipts")]
        public async Task<IActionResult> ImportDbfReceipts(IFormFile file, [FromForm] int? organizationId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пуст");
            }

            try
            {
                using var stream = file.OpenReadStream();
                // Временно используем тот же обработчик до выделения парсера приходов
                var result = await _dbfImportService.ImportDbfDataAsync(stream, file.FileName, organizationId);
                if (result.IsSuccess)
                {
                    return Ok(new { success = true, message = "Импорт DBF прихода завершен", importLogId = result.ImportLogId, totalRecords = result.TotalRecords, processedRecords = result.ProcessedRecords, successRecords = result.SuccessRecords, errorRecords = result.ErrorRecords, organizationId });
                }
                return BadRequest(new { success = false, message = "Ошибка импорта DBF прихода", errors = result.Errors });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте DBF прихода {FileName}", file.FileName);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost("json")]
        public async Task<IActionResult> ImportJsonData(IFormFile file, [FromForm] int? educationDepartmentId, [FromForm] int? organizationId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран или пуст");
            }

            if (!educationDepartmentId.HasValue)
            {
                return BadRequest("Необходимо выбрать УО");
            }

            if (!file.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Поддерживаются только JSON файлы");
            }

            try
            {
                _logger.LogInformation("Импорт справочника СВС из файла {FileName} для УО {EducationDepartmentId} и организации {OrganizationId}", file.FileName, educationDepartmentId, organizationId);
                
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                var jsonContent = await reader.ReadToEndAsync();

                var result = await _dataImportService.ImportJsonDataAsync(jsonContent, file.FileName, educationDepartmentId, organizationId);

                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        importLogId = result.ImportLogId,
                        totalRecords = result.TotalRecords,
                        processedRecords = result.ProcessedRecords,
                        successRecords = result.SuccessRecords,
                        errorRecords = result.ErrorRecords,
                        educationDepartmentId = educationDepartmentId
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message,
                        errors = result.Errors,
                        importLogId = result.ImportLogId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте справочника СВС");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }
    }
}
