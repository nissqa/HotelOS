using HotelOS.Data;
using HotelOS.DTOs;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace HotelOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
  

    public class CleaningTasksController : ControllerBase
    {
        private readonly HotelDbContext _context;


    public CleaningTasksController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CleaningTaskDto>>> GetCleaningTasks()
        {
            var tasks = await _context.CleaningTasks
                .Select(t => new CleaningTaskDto
                {
                    Id = t.Id,
                    RoomId = t.RoomId,
                    EmployeeId = t.EmployeeId,
                    TaskDate = t.TaskDate,
                    Status = t.Status,
                    Notes = t.Notes
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CleaningTaskDto>> GetCleaningTask(int id)
        {
            var task = await _context.CleaningTasks
                .Where(t => t.Id == id)
                .Select(t => new CleaningTaskDto
                {
                    Id = t.Id,
                    RoomId = t.RoomId,
                    EmployeeId = t.EmployeeId,
                    TaskDate = t.TaskDate,
                    Status = t.Status,
                    Notes = t.Notes
                })
                .FirstOrDefaultAsync();

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCleaningTask(CleaningTaskCreateDto dto)
        {
            var roomExists = await _context.Rooms
                .AnyAsync(r => r.Id == dto.RoomId);

            var employeeExists = await _context.Employees
                .AnyAsync(e => e.Id == dto.EmployeeId);

            if (!roomExists)
                return BadRequest("Oda bulunamadı.");

            if (!employeeExists)
                return BadRequest("Çalışan bulunamadı.");

            var task = new CleaningTask
            {
                RoomId = dto.RoomId,
                EmployeeId = dto.EmployeeId,
                TaskDate = dto.TaskDate,
                Status = dto.Status,
                Notes = dto.Notes
            };

            _context.CleaningTasks.Add(task);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Temizlik görevi oluşturuldu.",
                id = task.Id
            });
        }

        [HttpPut("{id}/start")]
        public async Task<IActionResult> StartCleaning(int id)
        {
            var task = await _context.CleaningTasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound("Temizlik görevi bulunamadı.");

            if (task.Status != "Bekliyor")
                return BadRequest("Bu görev başlatılamaz.");

            task.Status = "Temizlikte";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Temizlik başlatıldı."
            });
        }

        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteCleaning(int id)
        {
            var task = await _context.CleaningTasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound("Temizlik görevi bulunamadı.");

            if (task.Status != "Temizlikte")
                return BadRequest("Bu görev tamamlanamaz.");

            task.Status = "Temizlendi";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Temizlik tamamlandı."
            });
        }

        [HttpPut("{id}/maintenance")]



        public async Task<IActionResult> SendToMaintenance(int id, SendToMaintenanceDto dto)
        {
            var task = await _context.CleaningTasks
                .Include(t => t.Room)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound("Temizlik görevi bulunamadı.");

            if (task.Status != "Bekliyor" && task.Status != "Temizlikte")
                return BadRequest("Bu görev bakım için gönderilemez.");

            var allowedPriorities = new[] { "Normal", "Acil", "Çok Acil" };
            var priority = allowedPriorities.Contains(dto.Priority) ? dto.Priority : "Normal";


            var existingRequest = await _context.MaintenanceRequests
                .AnyAsync(m =>
                    m.RoomId == task.RoomId &&
                    m.Status != "Tamamlandı" &&
                    m.Status != "İptal");

            if (!existingRequest)
            {
                var maintenanceRequest = new MaintenanceRequest
                {
                    RoomId = task.RoomId,
                    EmployeeId = null,
                    Title = "Temizlik sırasında bakım ihtiyacı",
                    Description = string.IsNullOrWhiteSpace(task.Notes)
                        ? $"Oda {task.Room?.RoomNumber} için temizlik sırasında bakım ihtiyacı bildirildi."
                        : task.Notes,
                    Priority = priority, 
                    CreatedAt = DateTime.UtcNow
                };

                _context.MaintenanceRequests.Add(maintenanceRequest);
            }

            task.Status = "Bakım";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Oda bakım ekibine gönderildi."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCleaningTask(int id)
        {
            var task = await _context.CleaningTasks
                .FindAsync(id);

            if (task == null)
                return NotFound();

            _context.CleaningTasks.Remove(task);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }


}
