using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExchangeSystem.Services;
using ExchangeSystem.Data;
using ExchangeSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DataController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IStoreService _storeService;
        private readonly ITransactionService _transactionService;
        private readonly IDataJoinService _dataJoinService;
        private readonly ExchangeDbContext _context;
        private readonly ILogger<DataController> _logger;

        public DataController(
            IProductService productService,
            IStoreService storeService,
            ITransactionService transactionService,
            IDataJoinService dataJoinService,
            ExchangeDbContext context,
            ILogger<DataController> logger)
        {
            _productService = productService;
            _storeService = storeService;
            _transactionService = transactionService;
            _dataJoinService = dataJoinService;
            _context = context;
            _logger = logger;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromQuery] string? search = null)
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                
                // Filter by search term if provided
                if (!string.IsNullOrEmpty(search))
                {
                    products = products.Where(p => 
                        p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.Code.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.Category.Contains(search, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }
                
                return Ok(new { success = true, data = products });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, new { success = false, message = "Ошибка при получении продуктов" });
            }
        }

        [HttpGet("stores")]
        public async Task<IActionResult> GetStores()
        {
            try
            {
                var stores = await _storeService.GetAllStoresAsync();
                return Ok(new { success = true, data = stores });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stores");
                return StatusCode(500, new { success = false, message = "Ошибка при получении магазинов" });
            }
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions()
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactionsAsync();
                return Ok(new { success = true, data = transactions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transactions");
                return StatusCode(500, new { success = false, message = "Ошибка при получении транзакций" });
            }
        }

        [HttpGet("consumption")]
        public async Task<IActionResult> GetConsumptionData(
            [FromQuery] int? organizationId,
            [FromQuery] DateTime? date,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            try
            {
                if (!organizationId.HasValue)
                {
                    return BadRequest(new { success = false, message = "Необходимо указать organizationId" });
                }

                var query = _context.ProductConsumptions
                    .Include(pc => pc.Product)
                    .Where(pc => pc.OrganizationId == organizationId.Value);

                if (date.HasValue)
                {
                    var targetDate = DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
                    query = query.Where(pc => pc.ConsumptionDate.Date == targetDate.Date);
                }
                else if (from.HasValue || to.HasValue)
                {
                    if (from.HasValue)
                    {
                        var f = DateTime.SpecifyKind(from.Value, DateTimeKind.Utc);
                        query = query.Where(pc => pc.ConsumptionDate >= f);
                    }
                    if (to.HasValue)
                    {
                        var t = DateTime.SpecifyKind(to.Value, DateTimeKind.Utc);
                        query = query.Where(pc => pc.ConsumptionDate <= t);
                    }
                }

                var items = await query
                    .OrderBy(pc => pc.ConsumptionDate)
                    .ToListAsync();

                // Получаем сопоставления СВС
                var productIds = items.Select(i => i.ProductId).Distinct().ToList();
                var svsMappings = await _context.OrganizationProducts
                    .Where(op => op.OrganizationId == organizationId.Value && productIds.Contains(op.ProductId))
                    .ToDictionaryAsync(op => op.ProductId, op => op.SvsCode);

                // Автоматически создаем сопоставления для продуктов, у которых есть глобальный SvsCode, но нет в OrganizationProduct
                var productsWithoutOrgMapping = items
                    .Where(i => !svsMappings.ContainsKey(i.ProductId) && 
                                i.Product != null && 
                                !string.IsNullOrEmpty(i.Product.SvsCode))
                    .Select(i => i.Product)
                    .Distinct()
                    .ToList();

                if (productsWithoutOrgMapping.Any())
                {
                    foreach (var product in productsWithoutOrgMapping)
                    {
                        var orgProduct = new OrganizationProduct
                        {
                            OrganizationId = organizationId.Value,
                            ProductId = product.Id,
                            SvsCode = product.SvsCode,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.OrganizationProducts.Add(orgProduct);
                        svsMappings[product.Id] = product.SvsCode;
                    }
                    await _context.SaveChangesAsync();
                }

                var result = items.Select(i => new
                {
                    id = i.Id,
                    date = i.ConsumptionDate,
                    productId = i.ProductId,
                    productName = i.ProductName,
                    productCode = i.Product?.Code ?? "",
                    unit = i.Unit,
                    quantity = i.Quantity,
                    price = i.Price,
                    totalCost = i.TotalCost,
                    categoryId = i.CategoryId,
                    categoryName = i.CategoryName,
                    svsCode = svsMappings.TryGetValue(i.ProductId, out var code) ? code : i.Product?.SvsCode
                }).ToList();

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting consumption data");
                return StatusCode(500, new { success = false, message = "Ошибка при получении данных расхода" });
            }
        }

        [HttpGet("receipts")]
        public async Task<IActionResult> GetReceiptData(
            [FromQuery] int? organizationId,
            [FromQuery] DateTime? date,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            try
            {
                if (!organizationId.HasValue)
                {
                    return BadRequest(new { success = false, message = "Необходимо указать organizationId" });
                }

                var query = _context.ProductReceipts
                    .Include(pr => pr.Product)
                    .Where(pr => pr.OrganizationId == organizationId.Value);

                if (date.HasValue)
                {
                    var targetDate = DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
                    query = query.Where(pr => pr.ReceiptDate.Date == targetDate.Date);
                }
                else if (from.HasValue || to.HasValue)
                {
                    if (from.HasValue)
                    {
                        var f = DateTime.SpecifyKind(from.Value, DateTimeKind.Utc);
                        query = query.Where(pr => pr.ReceiptDate >= f);
                    }
                    if (to.HasValue)
                    {
                        var t = DateTime.SpecifyKind(to.Value, DateTimeKind.Utc);
                        query = query.Where(pr => pr.ReceiptDate <= t);
                    }
                }

                var items = await query
                    .OrderBy(pr => pr.ReceiptDate)
                    .ToListAsync();

                // Получаем сопоставления СВС
                var productIds = items.Select(i => i.ProductId).Distinct().ToList();
                var svsMappings = await _context.OrganizationProducts
                    .Where(op => op.OrganizationId == organizationId.Value && productIds.Contains(op.ProductId))
                    .ToDictionaryAsync(op => op.ProductId, op => op.SvsCode);

                // Автоматически создаем сопоставления для продуктов, у которых есть глобальный SvsCode, но нет в OrganizationProduct
                var productsWithoutOrgMapping = items
                    .Where(i => !svsMappings.ContainsKey(i.ProductId) && 
                                i.Product != null && 
                                !string.IsNullOrEmpty(i.Product.SvsCode))
                    .Select(i => i.Product)
                    .Distinct()
                    .ToList();

                if (productsWithoutOrgMapping.Any())
                {
                    foreach (var product in productsWithoutOrgMapping)
                    {
                        var orgProduct = new OrganizationProduct
                        {
                            OrganizationId = organizationId.Value,
                            ProductId = product.Id,
                            SvsCode = product.SvsCode,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.OrganizationProducts.Add(orgProduct);
                        svsMappings[product.Id] = product.SvsCode;
                    }
                    await _context.SaveChangesAsync();
                }

                var result = items.Select(i => new
                {
                    id = i.Id,
                    date = i.ReceiptDate,
                    documentNumber = i.DocumentNumber,
                    supplierName = i.SupplierName,
                    supplierUnp = i.SupplierUnp,
                    contractDate = i.ContractDate,
                    contractNumber = i.ContractNumber,
                    productId = i.ProductId,
                    productName = i.ProductName,
                    productCode = i.Product?.Code ?? "",
                    unit = i.Unit,
                    quantity = i.Quantity,
                    price = i.Price,
                    totalCost = i.TotalCost,
                    svsCode = svsMappings.TryGetValue(i.ProductId, out var code) ? code : i.Product?.SvsCode
                }).ToList();

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting receipt data");
                return StatusCode(500, new { success = false, message = "Ошибка при получении данных прихода" });
            }
        }

        [HttpPut("consumption/{id}/svs-code")]
        public async Task<IActionResult> UpdateConsumptionSvsCode(int id, [FromBody] UpdateSvsCodeRequest request)
        {
            try
            {
                var consumption = await _context.ProductConsumptions
                    .Include(pc => pc.Product)
                    .FirstOrDefaultAsync(pc => pc.Id == id);

                if (consumption == null)
                {
                    return NotFound(new { success = false, message = "Запись расхода не найдена" });
                }

                // Сохраняем сопоставление в OrganizationProducts
                if (consumption.OrganizationId.HasValue)
                {
                    var orgProduct = await _context.OrganizationProducts
                        .FirstOrDefaultAsync(op => op.OrganizationId == consumption.OrganizationId.Value && op.ProductId == consumption.ProductId);

                    if (orgProduct != null)
                    {
                        orgProduct.SvsCode = request.SvsCode;
                        orgProduct.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        orgProduct = new OrganizationProduct
                        {
                            OrganizationId = consumption.OrganizationId.Value,
                            ProductId = consumption.ProductId,
                            SvsCode = request.SvsCode,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.OrganizationProducts.Add(orgProduct);
                    }

                    await _context.SaveChangesAsync();
                }

                return Ok(new { success = true, message = "Код СВС обновлен" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating consumption SVS code");
                return StatusCode(500, new { success = false, message = "Ошибка при обновлении кода СВС" });
            }
        }

        [HttpPut("receipts/{id}/svs-code")]
        public async Task<IActionResult> UpdateReceiptSvsCode(int id, [FromBody] UpdateSvsCodeRequest request)
        {
            try
            {
                var receipt = await _context.ProductReceipts
                    .Include(pr => pr.Product)
                    .FirstOrDefaultAsync(pr => pr.Id == id);

                if (receipt == null)
                {
                    return NotFound(new { success = false, message = "Запись прихода не найдена" });
                }

                // Сохраняем сопоставление в OrganizationProducts
                if (receipt.OrganizationId.HasValue)
                {
                    var orgProduct = await _context.OrganizationProducts
                        .FirstOrDefaultAsync(op => op.OrganizationId == receipt.OrganizationId.Value && op.ProductId == receipt.ProductId);

                    if (orgProduct != null)
                    {
                        orgProduct.SvsCode = request.SvsCode;
                        orgProduct.UpdatedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        orgProduct = new OrganizationProduct
                        {
                            OrganizationId = receipt.OrganizationId.Value,
                            ProductId = receipt.ProductId,
                            SvsCode = request.SvsCode,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _context.OrganizationProducts.Add(orgProduct);
                    }

                    await _context.SaveChangesAsync();
                }

                return Ok(new { success = true, message = "Код СВС обновлен" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating receipt SVS code");
                return StatusCode(500, new { success = false, message = "Ошибка при обновлении кода СВС" });
            }
        }

        [HttpGet("joined")]
        public async Task<IActionResult> GetJoinedData([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
        {
            try
            {
                var joinedData = await _dataJoinService.GetJoinedDataAsync(pageSize, pageNumber);
                
                // Use reflection to access properties safely
                var dataType = joinedData.GetType();
                var records = dataType.GetProperty("records")?.GetValue(joinedData);
                var totalCount = dataType.GetProperty("totalCount")?.GetValue(joinedData);
                var pageNum = dataType.GetProperty("pageNumber")?.GetValue(joinedData);
                var pageSz = dataType.GetProperty("pageSize")?.GetValue(joinedData);
                var totalPages = dataType.GetProperty("totalPages")?.GetValue(joinedData);
                
                return Ok(new { 
                    success = true, 
                    data = records,
                    pagination = new {
                        totalCount = totalCount,
                        pageNumber = pageNum,
                        pageSize = pageSz,
                        totalPages = totalPages
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting joined data");
                return StatusCode(500, new { success = false, message = "Ошибка при получении объединенных данных" });
            }
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinData()
        {
            try
            {
                var result = await _dataJoinService.JoinDataAsync();
                return Ok(new { success = true, message = "Данные успешно объединены", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining data");
                return StatusCode(500, new { success = false, message = "Ошибка при объединении данных" });
            }
        }

        [HttpPost("regenerate-keys")]
        public async Task<IActionResult> RegenerateKeys()
        {
            try
            {
                var result = await _dataJoinService.RegenerateKeysAsync();
                return Ok(new { success = true, message = "Ключи успешно перегенерированы", data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating keys");
                return StatusCode(500, new { success = false, message = "Ошибка при перегенерации ключей" });
            }
        }

        // Product CRUD endpoints
        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { success = false, message = "Продукт не найден" });
                }
                return Ok(new { success = true, data = product });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product {ProductId}", id);
                return StatusCode(500, new { success = false, message = "Ошибка при получении продукта" });
            }
        }

        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] Models.Product product)
        {
            try
            {
                var createdProduct = await _productService.CreateProductAsync(product);
                return Ok(new { success = true, data = createdProduct, message = "Продукт создан успешно" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, new { success = false, message = "Ошибка при создании продукта" });
            }
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Models.Product product)
        {
            try
            {
                if (id != product.Id)
                {
                    return BadRequest(new { success = false, message = "ID продукта не совпадает" });
                }

                var updatedProduct = await _productService.UpdateProductAsync(product);
                return Ok(new { success = true, data = updatedProduct, message = "Продукт обновлен успешно" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", id);
                return StatusCode(500, new { success = false, message = "Ошибка при обновлении продукта" });
            }
        }

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var success = await _productService.DeleteProductAsync(id);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Продукт не найден" });
                }
                return Ok(new { success = true, message = "Продукт удален успешно" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                return StatusCode(500, new { success = false, message = "Ошибка при удалении продукта" });
            }
        }

        // Store CRUD endpoints
        [HttpGet("stores/{id}")]
        public async Task<IActionResult> GetStore(int id)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(id);
                if (store == null)
                {
                    return NotFound(new { success = false, message = "Магазин не найден" });
                }
                return Ok(new { success = true, data = store });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store {StoreId}", id);
                return StatusCode(500, new { success = false, message = "Ошибка при получении магазина" });
            }
        }

        [HttpPost("stores")]
        public async Task<IActionResult> CreateStore([FromBody] Models.Store store)
        {
            try
            {
                var createdStore = await _storeService.CreateStoreAsync(store);
                return Ok(new { success = true, data = createdStore, message = "Магазин создан успешно" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating store");
                return StatusCode(500, new { success = false, message = "Ошибка при создании магазина" });
            }
        }

        [HttpPut("stores/{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] Models.Store store)
        {
            try
            {
                if (id != store.Id)
                {
                    return BadRequest(new { success = false, message = "ID магазина не совпадает" });
                }

                var updatedStore = await _storeService.UpdateStoreAsync(store);
                return Ok(new { success = true, data = updatedStore, message = "Магазин обновлен успешно" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store {StoreId}", id);
                return StatusCode(500, new { success = false, message = "Ошибка при обновлении магазина" });
            }
        }

        [HttpDelete("stores/{id}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            try
            {
                var success = await _storeService.DeleteStoreAsync(id);
                if (!success)
                {
                    return NotFound(new { success = false, message = "Магазин не найден" });
                }
                return Ok(new { success = true, message = "Магазин удален успешно" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting store {StoreId}", id);
                return StatusCode(500, new { success = false, message = "Ошибка при удалении магазина" });
            }
        }
    }

    public class UpdateSvsCodeRequest
    {
        public string? SvsCode { get; set; }
    }
}
