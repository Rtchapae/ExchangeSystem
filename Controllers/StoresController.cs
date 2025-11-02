using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExchangeSystem.Services;
using ExchangeSystem.Models;

namespace ExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StoresController : ControllerBase
    {
        private readonly IStoreService _storeService;
        private readonly ILogger<StoresController> _logger;

        public StoresController(IStoreService storeService, ILogger<StoresController> logger)
        {
            _storeService = storeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetStores([FromQuery] string? search = null)
        {
            try
            {
                var stores = string.IsNullOrEmpty(search) 
                    ? await _storeService.GetAllStoresAsync()
                    : await _storeService.SearchStoresAsync(search);

                return Ok(new
                {
                    success = true,
                    data = stores
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении магазинов");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStore(int id)
        {
            try
            {
                var store = await _storeService.GetStoreByIdAsync(id);
                if (store == null)
                {
                    return NotFound(new { success = false, message = "Магазин не найден" });
                }

                return Ok(new
                {
                    success = true,
                    data = store
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении магазина {StoreId}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("by-education-department/{educationDepartmentId}")]
        public async Task<IActionResult> GetStoresByEducationDepartment(int educationDepartmentId)
        {
            try
            {
                var stores = await _storeService.GetStoresByEducationDepartmentAsync(educationDepartmentId);

                return Ok(new
                {
                    success = true,
                    data = stores
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении магазинов для УО {EducationDepartmentId}", educationDepartmentId);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateStore([FromBody] Store store)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Неверные данные", errors = ModelState });
                }

                var createdStore = await _storeService.CreateStoreAsync(store);
                return CreatedAtAction(nameof(GetStore), new { id = createdStore.Id }, new
                {
                    success = true,
                    message = "Магазин создан",
                    data = createdStore
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании магазина");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, [FromBody] Store store)
        {
            try
            {
                if (id != store.Id)
                {
                    return BadRequest(new { success = false, message = "Неверный ID магазина" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Неверные данные", errors = ModelState });
                }

                var updatedStore = await _storeService.UpdateStoreAsync(store);
                return Ok(new
                {
                    success = true,
                    message = "Магазин обновлен",
                    data = updatedStore
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении магазина {StoreId}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            try
            {
                var result = await _storeService.DeleteStoreAsync(id);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Магазин не найден" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Магазин удален"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении магазина {StoreId}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }
    }
}


