using Microsoft.EntityFrameworkCore;
using ExchangeSystem.Data;
using ExchangeSystem.Models;
using System.Text.Json;

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
                    await ProcessConsumptionData(parseResult.ParsedData.Cast<ConsumptionDataRow>().ToList(), importLog.Id, organizationId);
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
                    IsSuccess = importLog.SuccessRecords > 0, // Успех если есть хотя бы одна успешная запись
                    ImportLogId = importLog.Id,
                    TotalRecords = importLog.TotalRecords,
                    ProcessedRecords = importLog.ProcessedRecords,
                    SuccessRecords = importLog.SuccessRecords,
                    ErrorRecords = importLog.ErrorRecords,
                    Errors = parseResult.Errors,
                    Message = importLog.SuccessRecords > 0 ? 
                        (importLog.ErrorRecords > 0 ? "Импорт завершен с ошибками" : "Импорт успешно завершен") : 
                        "Импорт не удался"
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
                    await ProcessReceiptData(parseResult.ParsedData.Cast<ReceiptDataRow>().ToList(), importLog.Id, organizationId);
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

        private async Task ProcessConsumptionData(List<ConsumptionDataRow> data, int importLogId, int? organizationId)
        {
            int? educationDepartmentId = null;
            if (organizationId.HasValue)
            {
                var store = await _context.Stores.FirstOrDefaultAsync(s => s.Id == organizationId.Value);
                educationDepartmentId = store?.EducationDepartmentId;
            }
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
                    
                    // Пропускаем строки без продукта (например, "Детодни")
                    if (product == null)
                    {
                        continue;
                    }

                    // Сохранение данных по яслям
                    if (row.NurseryQuantity > 0 && nurseryCategory != null)
                    {
                        var consumption = new ProductConsumption
                        {
                            ConsumptionDate = DateTime.SpecifyKind(row.ConsumptionDate, DateTimeKind.Utc),
                            ProductId = product.Id,
                            ProductName = row.ProductName,
                            Unit = row.Unit,
                            Quantity = row.NurseryQuantity,
                            Price = row.NurseryCost / row.NurseryQuantity,
                            TotalCost = row.NurseryCost,
                            OrganizationId = organizationId,
                            EducationDepartmentId = educationDepartmentId,
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
                            ConsumptionDate = DateTime.SpecifyKind(row.ConsumptionDate, DateTimeKind.Utc),
                            ProductId = product.Id,
                            ProductName = row.ProductName,
                            Unit = row.Unit,
                            Quantity = row.KindergartenQuantity,
                            Price = row.KindergartenCost / row.KindergartenQuantity,
                            TotalCost = row.KindergartenCost,
                            OrganizationId = organizationId,
                            EducationDepartmentId = educationDepartmentId,
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
                            ConsumptionDate = DateTime.SpecifyKind(row.ConsumptionDate, DateTimeKind.Utc),
                            ProductId = product.Id,
                            ProductName = row.ProductName,
                            Unit = row.Unit,
                            Quantity = row.StaffQuantity,
                            Price = row.StaffCost / row.StaffQuantity,
                            TotalCost = row.StaffCost,
                            OrganizationId = organizationId,
                            EducationDepartmentId = educationDepartmentId,
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

        private async Task ProcessReceiptData(List<ReceiptDataRow> data, int importLogId, int? organizationId)
        {
            int? educationDepartmentId = null;
            if (organizationId.HasValue)
            {
                var store = await _context.Stores.FirstOrDefaultAsync(s => s.Id == organizationId.Value);
                educationDepartmentId = store?.EducationDepartmentId;
            }
            foreach (var row in data)
            {
                try
                {
                    // Создание или поиск продукта (Unit может быть пустым, возьмем из продукта)
                    var product = await FindOrCreateProductAsync(row.ProductCode, row.ProductName, row.Unit ?? "");
                    
                    // Пропускаем строки без продукта
                    if (product == null)
                    {
                        continue;
                    }

                    // Вычисляем цену из общей стоимости и количества
                    var price = row.Quantity > 0 ? row.TotalCost / row.Quantity : 0m;
                    var unit = !string.IsNullOrWhiteSpace(row.Unit) ? row.Unit : (product.Unit ?? "");

                    // Обрезаем строковые поля до максимальной длины
                    var documentNumber = !string.IsNullOrEmpty(row.DocumentNumber) 
                        ? (row.DocumentNumber.Length > 200 ? row.DocumentNumber.Substring(0, 200) : row.DocumentNumber)
                        : string.Empty;
                    var supplierName = !string.IsNullOrEmpty(row.SupplierName)
                        ? (row.SupplierName.Length > 140 ? row.SupplierName.Substring(0, 140) : row.SupplierName)
                        : string.Empty;
                    var contractNumber = !string.IsNullOrEmpty(row.ContractNumber)
                        ? (row.ContractNumber.Length > 200 ? row.ContractNumber.Substring(0, 200) : row.ContractNumber)
                        : null;
                    var supplierUnp = !string.IsNullOrEmpty(row.SupplierUnp)
                        ? (row.SupplierUnp.Length > 9 ? row.SupplierUnp.Substring(0, 9) : row.SupplierUnp)
                        : null;

                    // Создание записи прихода
                    var receipt = new ProductReceipt
                    {
                        ReceiptDate = DateTime.SpecifyKind(row.ReceiptDate, DateTimeKind.Utc),
                        DocumentNumber = documentNumber,
                        SupplierName = supplierName,
                        SupplierUnp = supplierUnp,
                        ContractDate = row.ContractDate.HasValue ? DateTime.SpecifyKind(row.ContractDate.Value, DateTimeKind.Utc) : null,
                        ContractNumber = contractNumber,
                        ProductId = product.Id,
                        ProductName = row.ProductName,
                        Unit = unit,
                        Quantity = row.Quantity,
                        Price = price,
                        TotalCost = row.TotalCost,
                        OrganizationId = organizationId,
                        EducationDepartmentId = educationDepartmentId,
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
            // Пропускаем строки без названия продукта (например, "Детодни")
            if (string.IsNullOrWhiteSpace(productName))
            {
                return null;
            }

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

        public async Task<ImportResult> ImportJsonDataAsync(string jsonContent, string fileName, int? educationDepartmentId = null, int? organizationId = null)
        {
            var importLog = new DataImportLog
            {
                FileName = fileName,
                ImportType = "JSON",
                ImportDate = DateTime.UtcNow,
                Status = "Processing"
            };

            _context.DataImportLogs.Add(importLog);
            await _context.SaveChangesAsync();

            try
            {
                var jsonData = JsonSerializer.Deserialize<JsonRoot>(jsonContent);
                if (jsonData?.MatItem == null)
                {
                    throw new InvalidOperationException("Неверный формат JSON файла");
                }

                var totalRecords = jsonData.MatItem.Count;
                var processedRecords = 0;
                var successRecords = 0;
                var errorRecords = 0;
                var errors = new List<string>();

                foreach (var item in jsonData.MatItem)
                {
                    try
                    {
                        var svsMaterial = new SvsMaterial
                        {
                            GroupMatId = item.GroupMatId,
                            MatId = item.MatId,
                            MeasureId = item.MeasureId,
                            NameGroupMat = item.NameGroupMat,
                            NameMat = item.NameMat,
                            NameMeasure = item.NameMeasure,
                            EducationDepartmentId = educationDepartmentId,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.SvsMaterials.Add(svsMaterial);
                        successRecords++;
                    }
                    catch (Exception ex)
                    {
                        errorRecords++;
                        errors.Add($"Ошибка при обработке материала {item.NameMat}: {ex.Message}");
                    }
                    processedRecords++;
                }

                await _context.SaveChangesAsync();

                importLog.Status = "Completed";
                importLog.TotalRecords = totalRecords;
                importLog.ProcessedRecords = processedRecords;
                importLog.SuccessRecords = successRecords;
                importLog.ErrorRecords = errorRecords;
                await _context.SaveChangesAsync();

                return new ImportResult
                {
                    IsSuccess = true,
                    ImportLogId = importLog.Id,
                    TotalRecords = totalRecords,
                    ProcessedRecords = processedRecords,
                    SuccessRecords = successRecords,
                    ErrorRecords = errorRecords,
                    Errors = errors,
                    Message = $"Импорт справочника СВС завершен. Обработано: {processedRecords}, Успешно: {successRecords}, Ошибок: {errorRecords}"
                };
            }
            catch (Exception ex)
            {
                importLog.Status = "Failed";
                importLog.ErrorMessage = ex.Message;
                await _context.SaveChangesAsync();

                return new ImportResult
                {
                    IsSuccess = false,
                    ImportLogId = importLog.Id,
                    Errors = new List<string> { ex.Message },
                    Message = "Ошибка при импорте справочника СВС"
                };
            }
        }
    }
}