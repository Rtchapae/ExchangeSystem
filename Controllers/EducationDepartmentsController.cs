using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExchangeSystem.Data;
using ExchangeSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace ExchangeSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationDepartmentsController : ControllerBase
    {
        private readonly ExchangeDbContext _context;
        private readonly ILogger<EducationDepartmentsController> _logger;

        public EducationDepartmentsController(ExchangeDbContext context, ILogger<EducationDepartmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetEducationDepartments()
        {
            try
            {
                var departments = await _context.EducationDepartments
                    .Where(ed => ed.IsActive)
                    .Include(ed => ed.Organizations)
                    .Select(ed => new
                    {
                        ed.Id,
                        ed.Name,
                        ed.City,
                        ed.Address,
                        ed.Phone,
                        ed.Email,
                        ed.DirectorName,
                        OrganizationsCount = ed.Organizations.Count(o => o.IsActive),
                        Organizations = ed.Organizations
                            .Where(o => o.IsActive)
                            .Select(o => new { o.Id, o.Name, o.City })
                            .ToList()
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = departments });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении УО");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEducationDepartment(int id)
        {
            try
            {
                var department = await _context.EducationDepartments
                    .Include(ed => ed.Organizations)
                    .FirstOrDefaultAsync(ed => ed.Id == id);

                if (department == null)
                {
                    return NotFound(new { success = false, message = "УО не найдено" });
                }

                return Ok(new { success = true, data = department });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении УО {Id}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEducationDepartment([FromBody] EducationDepartmentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Неверные данные", errors = ModelState });
                }

                var department = new EducationDepartment
                {
                    Name = dto.Name,
                    City = dto.City,
                    Address = dto.Address,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    DirectorName = dto.DirectorName,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.EducationDepartments.Add(department);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = department, message = "УО создано успешно" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании УО");
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEducationDepartment(int id, [FromBody] EducationDepartmentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Неверные данные", errors = ModelState });
                }

                var department = await _context.EducationDepartments.FindAsync(id);
                if (department == null)
                {
                    return NotFound(new { success = false, message = "УО не найдено" });
                }

                department.Name = dto.Name;
                department.City = dto.City;
                department.Address = dto.Address;
                department.Phone = dto.Phone;
                department.Email = dto.Email;
                department.DirectorName = dto.DirectorName;
                department.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { success = true, data = department, message = "УО обновлено успешно" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении УО {Id}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEducationDepartment(int id)
        {
            try
            {
                var department = await _context.EducationDepartments.FindAsync(id);
                if (department == null)
                {
                    return NotFound(new { success = false, message = "УО не найдено" });
                }

                // Проверяем, есть ли связанные организации
                var hasOrganizations = await _context.Stores.AnyAsync(s => s.EducationDepartmentId == id && s.IsActive);
                if (hasOrganizations)
                {
                    return BadRequest(new { success = false, message = "Нельзя удалить УО, к которому привязаны организации" });
                }

                department.IsActive = false;
                department.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "УО удалено успешно" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении УО {Id}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpGet("{id}/organizations")]
        public async Task<IActionResult> GetOrganizationsByDepartment(int id)
        {
            try
            {
                var organizations = await _context.Stores
                    .Where(s => s.EducationDepartmentId == id && s.IsActive)
                    .Select(s => new { s.Id, s.Name, s.City, s.Address, s.Phone, s.Manager })
                    .ToListAsync();

                return Ok(new { success = true, data = organizations });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении организаций УО {Id}", id);
                return StatusCode(500, new { success = false, message = "Внутренняя ошибка сервера" });
            }
        }
    }

    public class EducationDepartmentDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? DirectorName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
