using ExchangeSystem.Models;
using Microsoft.EntityFrameworkCore;
using ExchangeSystem.Data;
using System.Text.RegularExpressions;

namespace ExchangeSystem.Services
{
    public class ProductMappingService : IProductMappingService
    {
        private readonly ExchangeDbContext _context;
        private readonly ILogger<ProductMappingService> _logger;

        public ProductMappingService(ExchangeDbContext context, ILogger<ProductMappingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ProductMappingResult>> MapProductsToSvsAsync(List<SvsMaterialItem> svsMaterials, int? organizationId = null, bool autoSaveMappings = false)
        {
            var results = new List<ProductMappingResult>();
            
            // Получаем все продукты
            var products = await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync();

            _logger.LogInformation($"Начинаем сопоставление {products.Count} продуктов с {svsMaterials.Count} материалами СВС");

            foreach (var product in products)
            {
                var mappingResult = await MapSingleProductAsync(product, svsMaterials);
                
                // Если выбрана организация, проверяем существующие сопоставления
                if (organizationId.HasValue)
                {
                    var existingMapping = await _context.OrganizationProducts
                        .FirstOrDefaultAsync(op => op.OrganizationId == organizationId.Value && op.ProductId == product.Id);
                    
                    if (existingMapping != null)
                    {
                        // Если есть автоматическое сопоставление с кодом, обновляем существующее
                        if (mappingResult.IsAutoMapped && !string.IsNullOrEmpty(mappingResult.SvsCode))
                        {
                            // Обновляем только если код изменился или был пуст
                            if (existingMapping.SvsCode != mappingResult.SvsCode)
                            {
                                existingMapping.SvsCode = mappingResult.SvsCode;
                                existingMapping.UpdatedAt = DateTime.UtcNow;
                                await _context.SaveChangesAsync();
                                
                                _logger.LogInformation("Обновлено сопоставление для продукта '{ProductName}' (ID: {ProductId}) с кодом СВС {SvsCode} для организации {OrganizationId}", 
                                    product.Name, product.Id, mappingResult.SvsCode, organizationId);
                            }
                            mappingResult.IsAutoMapped = true;
                        }
                        else
                        {
                            mappingResult.SvsCode = existingMapping.SvsCode;
                            mappingResult.IsAutoMapped = false;
                        }
                    }
                    else if (mappingResult.IsAutoMapped && !string.IsNullOrEmpty(mappingResult.SvsCode) && autoSaveMappings)
                    {
                        // Автоматически сохраняем сопоставление для организации
                        var newMapping = new OrganizationProduct
                        {
                            OrganizationId = organizationId.Value,
                            ProductId = product.Id,
                            SvsCode = mappingResult.SvsCode,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.OrganizationProducts.Add(newMapping);
                        await _context.SaveChangesAsync();
                        
                        _logger.LogInformation("Автоматически сохранено сопоставление для продукта '{ProductName}' (ID: {ProductId}) с кодом СВС {SvsCode} для организации {OrganizationId}", 
                            product.Name, product.Id, mappingResult.SvsCode, organizationId);
                    }
                }
                else
                {
                    // Для общего справочника используем глобальный код СВС
                    if (mappingResult.IsAutoMapped && !string.IsNullOrEmpty(mappingResult.SvsCode))
                    {
                        product.SvsCode = mappingResult.SvsCode;
                        product.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        mappingResult.SvsCode = product.SvsCode;
                    }
                }

                results.Add(mappingResult);
            }

            return results;
        }

        public async Task<ProductMappingResult> MapSingleProductAsync(Product product, List<SvsMaterialItem> svsMaterials)
        {
            var result = new ProductMappingResult
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductCode = product.Code ?? "",
                SvsCode = product.SvsCode
            };

            // Ищем лучшее совпадение (проверяем и NameMat и NameGroupMat)
            var bestMatch = FindBestMatch(product.Name, product.Category, svsMaterials);
            
            if (bestMatch != null)
            {
                result.SvsMatId = bestMatch.MatId;
                result.SvsName = bestMatch.NameMat;
                result.SvsGroupId = bestMatch.GroupMatId;
                result.SvsGroupName = bestMatch.NameGroupMat;
                result.SvsMeasure = bestMatch.NameMeasure;
                result.IsAutoMapped = true;
                
                // Определяем уверенность на основе совпадения
                var nameMatConfidence = CalculateConfidence(product.Name, bestMatch.NameMat);
                var nameGroupMatConfidence = !string.IsNullOrWhiteSpace(product.Category) 
                    ? CalculateConfidence(product.Category, bestMatch.NameGroupMat)
                    : CalculateConfidence(product.Name, bestMatch.NameGroupMat);
                
                result.Confidence = Math.Max(nameMatConfidence, nameGroupMatConfidence);
                
                // Если есть совпадение по NameMat или NameGroupMat, автоматически устанавливаем MatId как SvsCode
                var normalizedProductName = NormalizeString(product.Name);
                var normalizedNameMat = NormalizeString(bestMatch.NameMat);
                var normalizedNameGroupMat = NormalizeString(bestMatch.NameGroupMat);
                var normalizedCategory = !string.IsNullOrWhiteSpace(product.Category) ? NormalizeString(product.Category) : null;
                
                // Проверяем точное совпадение (без учета регистра и пробелов)
                bool exactNameMatMatch = normalizedProductName.Equals(normalizedNameMat, StringComparison.OrdinalIgnoreCase) ||
                                        product.Name.Equals(bestMatch.NameMat, StringComparison.OrdinalIgnoreCase);
                
                bool exactNameGroupMatMatch = !string.IsNullOrWhiteSpace(product.Category) &&
                                             (normalizedCategory != null && normalizedCategory.Equals(normalizedNameGroupMat, StringComparison.OrdinalIgnoreCase) ||
                                              product.Category.Equals(bestMatch.NameGroupMat, StringComparison.OrdinalIgnoreCase));
                
                // Проверяем частичное совпадение (если одно название содержит другое)
                bool containsNameMat = normalizedProductName.Contains(normalizedNameMat, StringComparison.OrdinalIgnoreCase) ||
                                      normalizedNameMat.Contains(normalizedProductName, StringComparison.OrdinalIgnoreCase);
                
                bool containsNameGroupMat = (normalizedCategory != null && 
                                           (normalizedCategory.Contains(normalizedNameGroupMat, StringComparison.OrdinalIgnoreCase) ||
                                            normalizedNameGroupMat.Contains(normalizedCategory, StringComparison.OrdinalIgnoreCase))) ||
                                           normalizedProductName.Contains(normalizedNameGroupMat, StringComparison.OrdinalIgnoreCase) ||
                                           normalizedNameGroupMat.Contains(normalizedProductName, StringComparison.OrdinalIgnoreCase);
                
                // Проверяем схожесть (более низкий порог для установки кода)
                bool similarNameMat = CalculateSimilarity(normalizedProductName, normalizedNameMat) >= 0.6;
                bool similarNameGroupMat = (normalizedCategory != null && CalculateSimilarity(normalizedCategory, normalizedNameGroupMat) >= 0.6) ||
                                          CalculateSimilarity(normalizedProductName, normalizedNameGroupMat) >= 0.6;
                
                // Если есть любое совпадение - устанавливаем код
                if (exactNameMatMatch || exactNameGroupMatMatch || containsNameMat || containsNameGroupMat || similarNameMat || similarNameGroupMat)
                {
                    result.SvsCode = bestMatch.MatId.ToString();
                    
                    // Устанавливаем высокую уверенность для точных совпадений
                    if (exactNameMatMatch || exactNameGroupMatMatch)
                    {
                        result.Confidence = 1.0;
                    }
                    else if (containsNameMat || containsNameGroupMat)
                    {
                        result.Confidence = 0.9;
                    }
                    else
                    {
                        result.Confidence = Math.Max(nameMatConfidence, nameGroupMatConfidence);
                    }
                    
                    _logger.LogInformation("Автоматически сопоставлен продукт '{ProductName}' (ID: {ProductId}) с СВС '{SvsName}' (MatId: {MatId}), Confidence: {Confidence}", 
                        product.Name, product.Id, bestMatch.NameMat, bestMatch.MatId, result.Confidence);
                }
            }
            else
            {
                _logger.LogDebug("Не найдено совпадений для продукта '{ProductName}' (ID: {ProductId})", product.Name, product.Id);
            }

            return result;
        }

        private SvsMaterialItem? FindBestMatch(string productName, string? productCategory, List<SvsMaterialItem> svsMaterials)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return null;

            var normalizedProductName = NormalizeString(productName);
            var normalizedCategory = !string.IsNullOrWhiteSpace(productCategory) ? NormalizeString(productCategory) : null;

            var bestMatch = svsMaterials
                .Select(svs => new
                {
                    Item = svs,
                    // Проверяем совпадение с NameMat (приоритет выше)
                    NameMatScore = CalculateSimilarity(normalizedProductName, NormalizeString(svs.NameMat)),
                    // Проверяем совпадение с NameGroupMat
                    NameGroupMatScore = normalizedCategory != null 
                        ? CalculateSimilarity(normalizedCategory, NormalizeString(svs.NameGroupMat))
                        : CalculateSimilarity(normalizedProductName, NormalizeString(svs.NameGroupMat)),
                    // Проверяем точное совпадение
                    ExactNameMatMatch = productName.Equals(svs.NameMat, StringComparison.OrdinalIgnoreCase),
                    ExactNameGroupMatMatch = !string.IsNullOrWhiteSpace(productCategory) && 
                        productCategory.Equals(svs.NameGroupMat, StringComparison.OrdinalIgnoreCase)
                })
                .Select(x => new
                {
                    x.Item,
                    // Используем максимальный score, приоритет точному совпадению
                    Score = x.ExactNameMatMatch ? 1.0 : 
                           (x.ExactNameGroupMatMatch ? 0.95 :
                           Math.Max(x.NameMatScore, x.NameGroupMatScore * 0.8)) // NameMat имеет больший вес
                })
                .Where(x => x.Score > 0.2) // Понижаем минимальный порог схожести для лучшего поиска
                .OrderByDescending(x => x.Score)
                .FirstOrDefault();

            return bestMatch?.Item;
        }

        private double CalculateSimilarity(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
                return 0;

            // Простой алгоритм схожести на основе общих слов
            var words1 = str1.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var words2 = str2.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var commonWords = words1.Intersect(words2, StringComparer.OrdinalIgnoreCase).Count();
            var totalWords = Math.Max(words1.Length, words2.Length);

            if (totalWords == 0) return 0;

            var wordSimilarity = (double)commonWords / totalWords;

            // Дополнительная проверка на точное вхождение
            var exactMatch = str1.Contains(str2, StringComparison.OrdinalIgnoreCase) || 
                            str2.Contains(str1, StringComparison.OrdinalIgnoreCase);

            return exactMatch ? Math.Max(wordSimilarity, 0.9) : wordSimilarity;
        }

        private double CalculateConfidence(string productName, string svsName)
        {
            var similarity = CalculateSimilarity(NormalizeString(productName), NormalizeString(svsName));
            
            // Дополнительные факторы для повышения уверенности
            if (productName.Equals(svsName, StringComparison.OrdinalIgnoreCase))
                return 1.0;
            
            if (productName.Contains(svsName, StringComparison.OrdinalIgnoreCase) || 
                svsName.Contains(productName, StringComparison.OrdinalIgnoreCase))
                return Math.Min(similarity + 0.2, 1.0);

            return similarity;
        }

        private string NormalizeString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            // Убираем лишние символы, пробелы и приводим к нижнему регистру
            // Также убираем скобки и другие специальные символы
            var normalized = input.ToLowerInvariant()
                .Replace("(", " ")
                .Replace(")", " ")
                .Replace("-", " ")
                .Replace("_", " ");
            
            normalized = Regex.Replace(normalized, @"[^\w\s]", " ")
                .Replace("  ", " ")
                .Replace("  ", " ") // Двойной вызов для удаления всех двойных пробелов
                .Trim();

            return normalized;
        }

        public async Task<List<ProductMappingResult>> GetMappingResultsAsync(int? organizationId = null)
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync();

            var results = new List<ProductMappingResult>();

            foreach (var product in products)
            {
                var result = new ProductMappingResult
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductCode = product.Code ?? "",
                    SvsCode = product.SvsCode
                };

                if (organizationId.HasValue)
                {
                    var orgMapping = await _context.OrganizationProducts
                        .FirstOrDefaultAsync(op => op.OrganizationId == organizationId.Value && op.ProductId == product.Id);
                    
                    if (orgMapping != null)
                    {
                        result.SvsCode = orgMapping.SvsCode;
                    }
                }

                results.Add(result);
            }

            return results;
        }

        public async Task<bool> SaveMappingAsync(int productId, int svsMatId, string svsCode, int? organizationId = null)
        {
            try
            {
                if (organizationId.HasValue)
                {
                    // Сохраняем для конкретной организации
                    var existingMapping = await _context.OrganizationProducts
                        .FirstOrDefaultAsync(op => op.OrganizationId == organizationId.Value && op.ProductId == productId);

                    if (existingMapping != null)
                    {
                        existingMapping.SvsCode = svsCode;
                        existingMapping.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        var newMapping = new OrganizationProduct
                        {
                            OrganizationId = organizationId.Value,
                            ProductId = productId,
                            SvsCode = svsCode,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.OrganizationProducts.Add(newMapping);
                    }
                }
                else
                {
                    // Сохраняем глобально
                    var product = await _context.Products.FindAsync(productId);
                    if (product != null)
                    {
                        product.SvsCode = svsCode;
                        product.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении сопоставления продукта {ProductId}", productId);
                return false;
            }
        }

        public async Task<string> ExportMappingsToJsonAsync(int? organizationId = null)
        {
            var mappings = await GetMappingResultsAsync(organizationId);
            
            var exportData = new
            {
                ExportDate = DateTime.UtcNow,
                OrganizationId = organizationId,
                TotalMappings = mappings.Count,
                MappedCount = mappings.Count(m => !string.IsNullOrEmpty(m.SvsCode)),
                UnmappedCount = mappings.Count(m => string.IsNullOrEmpty(m.SvsCode)),
                Mappings = mappings.Select(m => new
                {
                    ProductId = m.ProductId,
                    ProductName = m.ProductName,
                    ProductCode = m.ProductCode,
                    SvsCode = m.SvsCode,
                    SvsName = m.SvsName,
                    SvsMatId = m.SvsMatId,
                    SvsGroupId = m.SvsGroupId,
                    SvsGroupName = m.SvsGroupName,
                    SvsMeasure = m.SvsMeasure,
                    IsAutoMapped = m.IsAutoMapped,
                    Confidence = m.Confidence,
                    MappingNotes = m.MappingNotes
                })
            };

            return System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

        public async Task<object> GetSvsStatsAsync()
        {
            try
            {
                var totalMaterials = await _context.SvsMaterialMappings.CountAsync();
                var mappedMaterials = await _context.SvsMaterialMappings.CountAsync(m => m.ProductId != null);
                var unmappedMaterials = totalMaterials - mappedMaterials;
                var autoMappedMaterials = await _context.SvsMaterialMappings.CountAsync(m => m.IsAutoMapped);

                return new
                {
                    totalMaterials,
                    mappedMaterials,
                    unmappedMaterials,
                    autoMappedMaterials,
                    mappingPercentage = totalMaterials > 0 ? (double)mappedMaterials / totalMaterials * 100 : 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статистики СВС");
                return new
                {
                    totalMaterials = 0,
                    mappedMaterials = 0,
                    unmappedMaterials = 0,
                    autoMappedMaterials = 0,
                    mappingPercentage = 0
                };
            }
        }

        public async Task<List<object>> GetSvsUpdatesAsync()
        {
            try
            {
                var updates = await _context.SvsCatalogUpdates
                    .Include(scu => scu.EducationDepartment)
                    .OrderByDescending(scu => scu.UpdateDate)
                    .Take(10)
                    .Select(scu => new
                    {
                        scu.Id,
                        scu.UpdateDate,
                        scu.TotalMaterials,
                        scu.MappedMaterials,
                        scu.UnmappedMaterials,
                        scu.UpdateSource,
                        scu.Notes,
                        EducationDepartmentName = scu.EducationDepartment != null ? scu.EducationDepartment.Name : null
                    })
                    .ToListAsync();

                return updates.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении обновлений СВС");
                return new List<object>();
            }
        }
    }
}
