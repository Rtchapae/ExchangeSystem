using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExchangeSystem.Services;

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
        private readonly ILogger<DataController> _logger;

        public DataController(
            IProductService productService,
            IStoreService storeService,
            ITransactionService transactionService,
            IDataJoinService dataJoinService,
            ILogger<DataController> logger)
        {
            _productService = productService;
            _storeService = storeService;
            _transactionService = transactionService;
            _dataJoinService = dataJoinService;
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
}