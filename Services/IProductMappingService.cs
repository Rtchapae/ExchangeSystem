using ExchangeSystem.Models;

namespace ExchangeSystem.Services
{
    public interface IProductMappingService
    {
        Task<List<ProductMappingResult>> MapProductsToSvsAsync(List<SvsMaterialItem> svsMaterials, int? organizationId = null, bool autoSaveMappings = false);
        Task<ProductMappingResult> MapSingleProductAsync(Product product, List<SvsMaterialItem> svsMaterials);
        Task<List<ProductMappingResult>> GetMappingResultsAsync(int? organizationId = null);
        Task<bool> SaveMappingAsync(int productId, int svsMatId, string svsCode, int? organizationId = null);
        Task<string> ExportMappingsToJsonAsync(int? organizationId = null);
        Task<object> GetSvsStatsAsync();
        Task<List<object>> GetSvsUpdatesAsync();
    }

    public class ProductMappingDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCategory { get; set; } = string.Empty;
        public string ProductUnit { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty; // Internal product code
        public string? SvsCode { get; set; } // SVS code for this product (can be organization specific)
        public string? SvsName { get; set; } // SVS name for this product
        public int? SvsMatId { get; set; } // SVS MatId
        public int? SvsGroupMatId { get; set; } // SVS GroupMatId
        public string? SvsMeasure { get; set; } // SVS Measure
        public decimal? LocalPrice { get; set; } // Organization-specific price
    }
}
