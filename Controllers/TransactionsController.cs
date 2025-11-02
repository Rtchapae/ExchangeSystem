using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExchangeSystem.Services;
using ExchangeSystem.Models;

namespace ExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] string? search = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] int? productId = null,
            [FromQuery] int? storeId = null)
        {
            try
            {
                var transactions = await _transactionService.GetAllTransactionsAsync();

                // Apply filters
                if (dateFrom.HasValue)
                {
                    transactions = transactions.Where(t => t.TransactionDate >= dateFrom.Value).ToList();
                }

                if (dateTo.HasValue)
                {
                    transactions = transactions.Where(t => t.TransactionDate <= dateTo.Value).ToList();
                }

                if (productId.HasValue)
                {
                    transactions = transactions.Where(t => t.ProductId == productId.Value).ToList();
                }

                if (storeId.HasValue)
                {
                    transactions = transactions.Where(t => t.StoreId == storeId.Value).ToList();
                }

                if (!string.IsNullOrEmpty(search))
                {
                    var searchLower = search.ToLower();
                    transactions = transactions.Where(t => 
                        (t.Product?.Name?.ToLower().Contains(searchLower) ?? false) ||
                        (t.Store?.Name?.ToLower().Contains(searchLower) ?? false) ||
                        (t.DocumentNumber?.ToLower().Contains(searchLower) ?? false) ||
                        (t.Supplier?.ToLower().Contains(searchLower) ?? false)
                    ).ToList();
                }

                return Ok(new
                {
                    success = true,
                    data = transactions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении транзакций");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction(int id)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionByIdAsync(id);
                if (transaction == null)
                {
                    return NotFound(new { success = false, message = "Транзакция не найдена" });
                }

                return Ok(new
                {
                    success = true,
                    data = transaction
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении транзакции {TransactionId}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] Transaction transaction)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Неверные данные", errors = ModelState });
                }

                var createdTransaction = await _transactionService.CreateTransactionAsync(transaction);
                return CreatedAtAction(nameof(GetTransaction), new { id = createdTransaction.Id }, new
                {
                    success = true,
                    message = "Транзакция создана",
                    data = createdTransaction
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании транзакции");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] Transaction transaction)
        {
            try
            {
                if (id != transaction.Id)
                {
                    return BadRequest(new { success = false, message = "Неверный ID транзакции" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Неверные данные", errors = ModelState });
                }

                var updatedTransaction = await _transactionService.UpdateTransactionAsync(transaction);
                return Ok(new
                {
                    success = true,
                    message = "Транзакция обновлена",
                    data = updatedTransaction
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении транзакции {TransactionId}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            try
            {
                var result = await _transactionService.DeleteTransactionAsync(id);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Транзакция не найдена" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Транзакция удалена"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении транзакции {TransactionId}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("by-product/{productId}")]
        public async Task<IActionResult> GetTransactionsByProduct(int productId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByProductAsync(productId);
                return Ok(new
                {
                    success = true,
                    data = transactions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении транзакций по продукту {ProductId}", productId);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("by-store/{storeId}")]
        public async Task<IActionResult> GetTransactionsByStore(int storeId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByStoreAsync(storeId);
                return Ok(new
                {
                    success = true,
                    data = transactions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении транзакций по магазину {StoreId}", storeId);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetTransactionsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByDateRangeAsync(startDate, endDate);
                return Ok(new
                {
                    success = true,
                    data = transactions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении транзакций по диапазону дат");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }
    }
}


