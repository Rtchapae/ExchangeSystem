using ExchangeSystem.Models;
using ExchangeSystem.Data;
using Microsoft.EntityFrameworkCore;
using ExcelDataReader;
using System.Text;

namespace ExchangeSystem.Services
{
    public interface IDbfImportService
    {
        Task<ImportResult> ImportDbfDataAsync(Stream fileStream, string fileName, int? organizationId = null);
    }

    public class DbfImportService : IDbfImportService
    {
        private readonly ExchangeDbContext _context;
        private readonly ILogger<DbfImportService> _logger;

        public DbfImportService(ExchangeDbContext context, ILogger<DbfImportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ImportResult> ImportDbfDataAsync(Stream fileStream, string fileName, int? organizationId = null)
        {
            var result = new ImportResult
            {
                IsSuccess = false,
                TotalRecords = 0,
                ProcessedRecords = 0,
                SuccessRecords = 0,
                ErrorRecords = 0,
                Errors = new List<string>()
            };

            try
            {
                // Создаем лог импорта
                var importLog = new DataImportLog
                {
                    FileName = fileName,
                    ImportType = "DbfData",
                    ImportDate = DateTime.UtcNow,
                    Status = "Processing"
                };

                _context.DataImportLogs.Add(importLog);
                await _context.SaveChangesAsync();

                // Читаем DBF файл как Excel (так как DBF может открываться в Excel)
                var dbfData = await ReadDbfFileAsync(fileStream);
                result.TotalRecords = dbfData.Count;

                _logger.LogInformation($"Начинаем импорт DBF файла: {fileName}, записей: {dbfData.Count}");

                foreach (var row in dbfData)
                {
                    try
                    {
                        await ProcessDbfRowAsync(row, organizationId, importLog.Id);
                        result.SuccessRecords++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при обработке строки DBF: {Product}", row.Product);
                        result.Errors.Add($"Продукт '{row.Product}': {ex.Message}");
                        result.ErrorRecords++;
                    }

                    result.ProcessedRecords++;
                }

                // Обновляем статус импорта
                importLog.TotalRecords = result.TotalRecords;
                importLog.ProcessedRecords = result.ProcessedRecords;
                importLog.SuccessRecords = result.SuccessRecords;
                importLog.ErrorRecords = result.ErrorRecords;
                importLog.Status = result.ErrorRecords > 0 ? "CompletedWithErrors" : "Completed";
                importLog.CompletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.ImportLogId = importLog.Id;

                _logger.LogInformation($"Импорт DBF завершен: успешно {result.SuccessRecords}, ошибок {result.ErrorRecords}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте DBF файла: {FileName}", fileName);
                result.Errors.Add($"Общая ошибка импорта: {ex.Message}");
            }

            return result;
        }

        private async Task<List<DbfDataRow>> ReadDbfFileAsync(Stream fileStream)
        {
            var data = new List<DbfDataRow>();

            try
            {
                // Настраиваем кодировку для корректного чтения русских символов
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using var reader = ExcelReaderFactory.CreateReader(fileStream);
                
                // Читаем данные построчно
                var rowNumber = 0;
                while (reader.Read())
                {
                    rowNumber++;
                    
                    // Пропускаем заголовок
                    if (rowNumber == 1) continue;

                    try
                    {
                        var dbfRow = new DbfDataRow
                        {
                            DateTime = ParseDateTime(reader.GetValue(0)?.ToString()),
                            Cipher = reader.GetValue(1)?.ToString() ?? "",
                            Product = reader.GetValue(2)?.ToString() ?? "",
                            EatingId = reader.GetValue(3)?.ToString() ?? "",
                            Eating = reader.GetValue(4)?.ToString() ?? "",
                            Measure = reader.GetValue(5)?.ToString() ?? "",
                            WeightTotal = ParseDecimal(reader.GetValue(6)?.ToString()),
                            EatingCount = ParseDecimal(reader.GetValue(7)?.ToString()),
                            PriceOneKg = ParseDecimal(reader.GetValue(8)?.ToString()),
                            TotalPrice = ParseDecimal(reader.GetValue(9)?.ToString())
                        };

                        // Проверяем обязательные поля
                        if (string.IsNullOrWhiteSpace(dbfRow.Product))
                        {
                            continue; // Пропускаем строки без названия продукта
                        }

                        data.Add(dbfRow);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Ошибка при парсинге строки {RowNumber} DBF файла", rowNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при чтении DBF файла");
                throw;
            }

            return data;
        }

        private async Task ProcessDbfRowAsync(DbfDataRow row, int? organizationId, int importLogId)
        {
            // Ищем или создаем продукт
            var product = await FindOrCreateProductFromDbfAsync(row);
            
            if (product == null)
            {
                throw new Exception($"Не удалось найти или создать продукт: {row.Product}");
            }

            // Создаем транзакцию потребления
            var transaction = new Transaction
            {
                ProductId = product.Id,
                StoreId = organizationId ?? 1, // Если организация не указана, используем первую
                Quantity = row.WeightTotal,
                Price = row.PriceOneKg,
                TransactionDate = row.DateTime,
                Notes = $"DBF импорт: {row.Eating}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        private async Task<Product?> FindOrCreateProductFromDbfAsync(DbfDataRow row)
        {
            // Сначала ищем по названию
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Name.Equals(row.Product, StringComparison.OrdinalIgnoreCase));

            if (product == null)
            {
                // Создаем новый продукт
                product = new Product
                {
                    Name = row.Product,
                    Code = row.Cipher,
                    Unit = row.Measure,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }

            return product;
        }

        private DateTime ParseDateTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DateTime.UtcNow;

            if (DateTime.TryParse(value, out var result))
                return result;

            return DateTime.UtcNow;
        }

        private decimal ParseDecimal(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            // Заменяем запятую на точку для корректного парсинга
            var normalizedValue = value.Replace(",", ".");
            
            if (decimal.TryParse(normalizedValue, out var result))
                return result;

            return 0;
        }
    }
}
