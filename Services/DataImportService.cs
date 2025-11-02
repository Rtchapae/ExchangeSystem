using Microsoft.EntityFrameworkCore;
using ExchangeSystem.Data;
using ExchangeSystem.Models;

namespace ExchangeSystem.Services
{
    public class DataImportService : IDataImportService
    {
        private readonly ExchangeDbContext _context;
        private readonly ICsvParserService _csvParser;
        private readonly ILogger<DataImportService> _logger;

        public DataImportService(
            ExchangeDbContext context,
            ICsvParserService csvParser,
            ILogger<DataImportService> logger)
        {
            _context = context;
            _csvParser = csvParser;
            _logger = logger;
        }

        public async Task<ImportResult> ImportConsumptionDataAsync(Stream csvStream, string fileName, int? organizationId = null)
        {
            var importLog = new DataImportLog
            {
                FileName = fileName,
                ImportType = "Consumption",
                ImportDate = DateTime.UtcNow,
                Status = "Processing"
            };

            _context.DataImportLogs.Add(importLog);
            await _context.SaveChangesAsync();

            try
            {
                var parseResult = await _csvParser.ParseConsumptionDataAsync(csvStream, fileName);
                
                importLog.TotalRecords = parseResult.TotalRows;
                importLog.ProcessedRecords = parseResult.ProcessedRows;
                importLog.ErrorRecords = parseResult.ErrorRows;
                importLog.SuccessRecords = parseResult.ProcessedRows - parseResult.ErrorRows;

                // Сохранение ошибок парсинга
                foreach (var error in parseResult.Errors)
                {
                    _context.DataImportErrors.Add(new DataImportError
                    {
                        ImportLogId = importLog.Id,
                        RowNumber = 0,
                        ErrorType = "ParseError",
                        ErrorMessage = error
                    });
                }

                // Обрабатываем данные даже если есть ошибки (частичный успех)
                if (parseResult.ParsedData.Any())
                {
                    await ProcessConsumptionData(parseResult.ParsedData.Cast<ConsumptionDataRow>().ToList(), importLog.Id);
                }

                // Статус зависит от наличия успешно обработанных записей
                if (importLog.SuccessRecords > 0 && importLog.ErrorRecords > 0)
                {
                    importLog.Status = "CompletedWithErrors";
                }
                else if (importLog.SuccessRecords > 0)
                {
                    importLog.Status = "Completed";
                }
                else
                {
                    importLog.Status = "Failed";
                }
                importLog.CompletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new ImportResult
                {
                    IsSuccess = parseResult.IsSuccess,
                    ImportLogId = importLog.Id,
                    TotalRecords = importLog.TotalRecords,
                    ProcessedRecords = importLog.ProcessedRecords,
                    SuccessRecords = importLog.SuccessRecords,
                    ErrorRecords = importLog.ErrorRecords,
                    Errors = parseResult.Errors,
                    Message = parseResult.IsSuccess ? "Импорт успешно завершен" : "Импорт завершен с ошибками"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте данных потребления из файла {FileName}", fileName);
                
                importLog.Status = "Failed";
                importLog.ErrorMessage = ex.Message;
                importLog.CompletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new ImportResult
                {
                    IsSuccess = false,
                    ImportLogId = importLog.Id,
                    Errors = new List<string> { ex.Message },
                    Message = "Ошибка при импорте данных"
                };
            }
        }

        public async Task<ImportResult> ImportProductDataAsync(Stream csvStream, string fileName)
        {
            var importLog = new DataImportLog
            {
                FileName = fileName,
                ImportType = "Products",
                ImportDate = DateTime.UtcNow,
                Status = "Processing"
            };

            _context.DataImportLogs.Add(importLog);
            await _context.SaveChangesAsync();

            try
            {
                var parseResult = await _csvParser.ParseProductDataAsync(csvStream, fileName);
                
                importLog.TotalRecords = parseResult.TotalRows;
                importLog.ProcessedRecords = parseResult.ProcessedRows;
                importLog.ErrorRecords = parseResult.ErrorRows;
                importLog.SuccessRecords = parseResult.ProcessedRows - parseResult.ErrorRows;

                // Сохранение ошибок парсинга
                foreach (var error in parseResult.Errors)
                {
                    _context.DataImportErrors.Add(new DataImportError
                    {
                        ImportLogId = importLog.Id,
                        RowNumber = 0,
                        ErrorType = "ParseError",
                        ErrorMessage = error
                    });
                }

                // Обрабатываем данные даже если есть ошибки
                if (parseResult.ParsedData.Any())
                {
                    await ProcessProductData(parseResult.ParsedData.Cast<ProductDataRow>().ToList(), importLog.Id);
                }

                // Статус зависит от наличия успешно обработанных записей
                if (importLog.SuccessRecords > 0 && importLog.ErrorRecords > 0)
                {
                    importLog.Status = "CompletedWithErrors";
                }
                else if (importLog.SuccessRecords > 0)
                {
                    importLog.Status = "Completed";
                }
                else
                {
                    importLog.Status = "Failed";
                }
                importLog.CompletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new ImportResult
                {
                    IsSuccess = parseResult.IsSuccess || importLog.SuccessRecords > 0,
                    ImportLogId = importLog.Id,
                    TotalRecords = importLog.TotalRecords,
                    ProcessedRecords = importLog.ProcessedRecords,
                    SuccessRecords = importLog.SuccessRecords,
                    ErrorRecords = importLog.ErrorRecords,
                    Errors = parseResult.Errors,
                    Message = importLog.SuccessRecords > 0 ? "Импорт успешно завершен" : "Импорт завершен с ошибками"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте данных продуктов из файла {FileName}", fileName);
                
                importLog.Status = "Failed";
                importLog.ErrorMessage = ex.Message;
                importLog.CompletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new ImportResult
                {
                    IsSuccess = false,
                    ImportLogId = importLog.Id,
                    Errors = new List<string> { ex.Message },
                    Message = "Ошибка при импорте данных"
                };
            }
        }

        public async Task<ImportResult> ImportReceiptDataAsync(Stream csvStream, string fileName, int? organizationId = null)
        {
            var importLog = new DataImportLog
            {
                FileName = fileName,
                ImportType = "Receipts",
                ImportDate = DateTime.UtcNow,
                Status = "Processing"
            };

            _context.DataImportLogs.Add(importLog);
            await _context.SaveChangesAsync();

            try
            {
                var parseResult = await _csvParser.ParseReceiptDataAsync(csvStream, fileName);
                
                importLog.TotalRecords = parseResult.TotalRows;
                importLog.ProcessedRecords = parseResult.ProcessedRows;
                importLog.ErrorRecords = parseResult.ErrorRows;
                importLog.SuccessRecords = parseResult.ProcessedRows - parseResult.ErrorRows;

                // Сохранение ошибок парсинга
                foreach (var error in parseResult.Errors)
                {
                    _context.DataImportErrors.Add(new DataImportError
                    {
                        ImportLogId = importLog.Id,
                        RowNumber = 0,
                        ErrorType = "ParseError",
                        ErrorMessage = error
                    });
                }

                // Обрабатываем данные даже если есть ошибки (частичный успех)
                if (parseResult.ParsedData.Any())
                {
                    await ProcessReceiptData(parseResult.ParsedData.Cast<ReceiptDataRow>().ToList(), importLog.Id);
                }

                // Статус зависит от наличия успешно обработанных записей
                if (importLog.SuccessRecords > 0 && importLog.ErrorRecords > 0)
                {
                    importLog.Status = "CompletedWithErrors";
                }
                else if (importLog.SuccessRecords > 0)
                {
                    importLog.Status = "Completed";
                }
                else
                {
                    importLog.Status = "Failed";
                }
                importLog.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

                return new ImportResult
                {
                    IsSuccess = parseResult.IsSuccess || importLog.SuccessRecords > 0,
                    ImportLogId = importLog.Id,
                    TotalRecords = importLog.TotalRecords,
                    ProcessedRecords = importLog.ProcessedRecords,
                    SuccessRecords = importLog.SuccessRecords,
                    ErrorRecords = importLog.ErrorRecords,
                    Errors = parseResult.Errors,
                    Message = importLog.SuccessRecords > 0 ? "Импорт успешно завершен" : "Импорт завершен с ошибками"
                };
                }
                catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте данных прихода из файла {FileName}", fileName);
                
                importLog.Status = "Failed";
                importLog.ErrorMessage = ex.Message;
                importLog.CompletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new ImportResult
                {
                    IsSuccess = false,
                    ImportLogId = importLog.Id,
                    Errors = new List<string> { ex.Message },
                    Message = "Ошибка при импорте данных"
                };
            }
        }

        private async Task ProcessConsumptionData(List<ConsumptionDataRow> data, int importLogId)
        {
            var categories = await _context.ConsumptionCategories.ToListAsync();
            var nurseryCategory = categories.FirstOrDefault(c => c.Code == "NURSERY");
            var kindergartenCategory = categories.FirstOrDefault(c => c.Code == "KINDERGARTEN");
            var staffCategory = categories.FirstOrDefault(c => c.Code == "STAFF");

            foreach (var row in data)
            {
                try
                {
                    // Создание или поиск продукта
                    var product = await FindOrCreateProductAsync(row.ProductCode, row.ProductName, row.Unit);

                    // Сохранение данных по яслям
                    if (row.NurseryQuantity > 0 && nurseryCategory != null)
                    {
                        var consumption = new ProductConsumption
                        {
                            ConsumptionDate = row.ConsumptionDate,
                        ProductId = product.Id,
                            ProductName = row.ProductName,
                            Unit = row.Unit,
                            Quantity = row.NurseryQuantity,
                            Price = row.NurseryCost / row.NurseryQuantity,
                            TotalCost = row.NurseryCost,
                            CategoryId = nurseryCategory.Id,
                            CategoryName = nurseryCategory.Name,
                            NurseryQuantity = row.NurseryQuantity,
                            NurseryCost = row.NurseryCost
                        };
                        _context.ProductConsumptions.Add(consumption);
                    }

                    // Сохранение данных по саду
                    if (row.KindergartenQuantity > 0 && kindergartenCategory != null)
                    {
                        var consumption = new ProductConsumption
                        {
                            ConsumptionDate = row.ConsumptionDate,
                            ProductId = product.Id,
                            ProductName = row.ProductName,
                            Unit = row.Unit,
                            Quantity = row.KindergartenQuantity,
                            Price = row.KindergartenCost / row.KindergartenQuantity,
                            TotalCost = row.KindergartenCost,
                            CategoryId = kindergartenCategory.Id,
                            CategoryName = kindergartenCategory.Name,
                            KindergartenQuantity = row.KindergartenQuantity,
                            KindergartenCost = row.KindergartenCost
                        };
                        _context.ProductConsumptions.Add(consumption);
                    }

                    // Сохранение данных по сотрудникам
                    if (row.StaffQuantity > 0 && staffCategory != null)
                    {
                        var consumption = new ProductConsumption
                        {
                            ConsumptionDate = row.ConsumptionDate,
                            ProductId = product.Id,
                            ProductName = row.ProductName,
                            Unit = row.Unit,
                            Quantity = row.StaffQuantity,
                            Price = row.StaffCost / row.StaffQuantity,
                            TotalCost = row.StaffCost,
                            CategoryId = staffCategory.Id,
                            CategoryName = staffCategory.Name,
                            StaffQuantity = row.StaffQuantity,
                            StaffCost = row.StaffCost
                        };
                        _context.ProductConsumptions.Add(consumption);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обработке строки данных потребления");
                    
                    _context.DataImportErrors.Add(new DataImportError
                    {
                        ImportLogId = importLogId,
                        RowNumber = 0,
                        ErrorType = "ProcessingError",
                        ErrorMessage = ex.Message
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task ProcessProductData(List<ProductDataRow> data, int importLogId)
        {
            var processedCodes = new HashSet<string>();
            
            foreach (var row in data)
            {
                try
                {
                    // Пропускаем продукты без названия
                    if (string.IsNullOrWhiteSpace(row.ProductName))
                    {
                        continue;
                    }

                    // Создаем уникальный код: если код повторяется, добавляем название
                    var uniqueCode = row.ProductCode;
                    if (processedCodes.Contains(row.ProductCode))
                    {
                        // Для дубликатов создаем уникальный код
                        uniqueCode = $"{row.ProductCode}_{row.ProductId}";
                    }
                    processedCodes.Add(row.ProductCode);

                    // Проверяем, существует ли продукт с таким кодом или названием
                    var existingProduct = await _context.Products
                        .FirstOrDefaultAsync(p => p.Code == uniqueCode || 
                                                  (p.Code == row.ProductCode && p.Name == row.ProductName));

                    if (existingProduct == null)
                    {
                        // Создаем новый продукт
                        var product = new Product
                        {
                            Name = row.ProductName,
                            Code = uniqueCode,
                            ExternalId = row.ProductId, // Внутренний ID из системы питания
                            Category = row.Category,
                            Unit = row.Unit,
                            IsActive = row.IsActive,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.Products.Add(product);
                        
                        // Сохраняем сразу, чтобы избежать конфликтов
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Обновляем существующий продукт
                        if (string.IsNullOrEmpty(existingProduct.ExternalId))
                        {
                            existingProduct.ExternalId = row.ProductId;
                        }
                        existingProduct.Category = row.Category;
                        existingProduct.Unit = row.Unit;
                        existingProduct.IsActive = row.IsActive;
                        existingProduct.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обработке продукта {ProductName}", row.ProductName);
                    
                    _context.DataImportErrors.Add(new DataImportError
                    {
                        ImportLogId = importLogId,
                        RowNumber = 0,
                        ErrorType = "ProcessingError",
                        ErrorMessage = $"{row.ProductName}: {ex.Message}"
                    });
                    await _context.SaveChangesAsync();
                }
            }
        }

        private async Task ProcessReceiptData(List<ReceiptDataRow> data, int importLogId)
        {
            foreach (var row in data)
            {
                try
                {
                    // Создание или поиск продукта
                    var product = await FindOrCreateProductAsync(row.ProductCode, row.ProductName, row.Unit);

                    // Создание записи прихода
                    var receipt = new ProductReceipt
                    {
                        ReceiptDate = row.ReceiptDate,
                        DocumentNumber = row.DocumentNumber,
                        SupplierName = row.SupplierName,
                        SupplierUnp = row.SupplierUnp,
                        ContractDate = row.ContractDate,
                        ContractNumber = row.ContractNumber,
                        ProductId = product.Id,
                        ProductName = row.ProductName,
                        Unit = row.Unit,
                        Quantity = row.Quantity,
                        Price = row.Price,
                        TotalCost = row.TotalCost,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.ProductReceipts.Add(receipt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обработке строки данных прихода");
                    
                    _context.DataImportErrors.Add(new DataImportError
                    {
                        ImportLogId = importLogId,
                        RowNumber = 0,
                        ErrorType = "ProcessingError",
                        ErrorMessage = ex.Message
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<Product> FindOrCreateProductAsync(string productCode, string productName, string unit)
        {
            // Сначала ищем по коду И названию (точное совпадение)
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Code == productCode && p.Name == productName);

            // Если не нашли, ищем только по названию
            if (product == null)
            {
                product = await _context.Products
                .FirstOrDefaultAsync(p => p.Name == productName);
            }

            // Если не нашли, ищем только по коду
            if (product == null && !string.IsNullOrWhiteSpace(productCode))
            {
                product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Code == productCode);
            }

            if (product == null)
            {
                // Создаем новый продукт
                // Если код уже существует, делаем его уникальным
                var existingCode = await _context.Products.AnyAsync(p => p.Code == productCode);
                var finalCode = existingCode ? $"{productCode}_{Guid.NewGuid().ToString().Substring(0, 8)}" : productCode;

                product = new Product
                {
                    Name = productName,
                    Code = finalCode,
                    Unit = unit,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Products.Add(product);
                
                try
                {
                await _context.SaveChangesAsync();
            }
                catch (DbUpdateException)
                {
                    // Если все равно ошибка дубликата, ищем существующий
                    _context.Entry(product).State = EntityState.Detached;
                    product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Code == productCode || p.Name == productName);
                    
                    if (product == null)
                    {
                        throw;
                    }
                }
            }

            return product;
        }

        public async Task<List<DataImportLog>> GetImportHistoryAsync()
        {
            return await _context.DataImportLogs
                .OrderByDescending(log => log.ImportDate)
                .ToListAsync();
        }

        public async Task<DataImportLog?> GetImportLogByIdAsync(int id)
        {
            return await _context.DataImportLogs
                .Include(log => log.Errors)
                .FirstOrDefaultAsync(log => log.Id == id);
        }

        public async Task<List<DataImportError>> GetImportErrorsAsync(int importLogId)
        {
            return await _context.DataImportErrors
                .Where(error => error.ImportLogId == importLogId)
                .OrderBy(error => error.RowNumber)
                .ToListAsync();
        }
    }
}