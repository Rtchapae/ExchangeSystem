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

        public async Task<List<ProductMappingResult>> MapProductsToSvsAsync(List<SvsMaterialItem> svsMaterials, int? organizationId = null)
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
                        mappingResult.SvsCode = existingMapping.SvsCode;
                        mappingResult.IsAutoMapped = false;
                    }
                }
                else
                {
                    // Для общего справочника используем глобальный код СВС
                    mappingResult.SvsCode = product.SvsCode;
                    mappingResult.IsAutoMapped = false;
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

            // Ищем лучшее совпадение
            var bestMatch = FindBestMatch(product.Name, svsMaterials);
            
            if (bestMatch != null)
            {
                result.SvsMatId = bestMatch.MatId;
                result.SvsName = bestMatch.NameMat;
                result.SvsGroupId = bestMatch.GroupMatId;
                result.SvsGroupName = bestMatch.NameGroupMat;
                result.SvsMeasure = bestMatch.NameMeasure;
                result.IsAutoMapped = true;
                result.Confidence = CalculateConfidence(product.Name, bestMatch.NameMat);
                
                // Если уверенность высокая, автоматически устанавливаем код СВС
                if (result.Confidence > 0.8)
                {
                    result.SvsCode = bestMatch.MatId.ToString();
                }
            }

            return result;
        }

        private SvsMaterialItem? FindBestMatch(string productName, List<SvsMaterialItem> svsMaterials)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return null;

            var normalizedProductName = NormalizeString(productName);
            var bestMatch = svsMaterials
                .Select(svs => new
                {
                    Item = svs,
                    Score = CalculateSimilarity(normalizedProductName, NormalizeString(svs.NameMat))
                })
                .Where(x => x.Score > 0.3) // Минимальный порог схожести
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

            // Убираем лишние символы и приводим к нижнему регистру
            return Regex.Replace(input.ToLowerInvariant(), @"[^\w\s]", " ")
                .Replace("  ", " ")
                .Trim();
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
