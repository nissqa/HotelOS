using HotelOS.Data;
using HotelOS.DTOs;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceRequestsController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public MaintenanceRequestsController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaintenancePanelDto>>> GetRequests()
        {
            var requests = await _context.MaintenanceRequests
                .Select(m => new MaintenancePanelDto
                {
                    Id = m.Id,
                    RoomId = m.RoomId,
                    EmployeeId = m.EmployeeId,
                    Title = m.Title,
                    Description = m.Description,
                    Priority = m.Priority,
                    Status = m.Status,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();

            return Ok(requests);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<MaintenancePanelDto>> GetRequest(int id)
        {
            var request = await _context.MaintenanceRequests
                .Where(m => m.Id == id)
                .Select(m => new MaintenancePanelDto
                {
                    Id = m.Id,
                    RoomId = m.RoomId,
                    EmployeeId = m.EmployeeId,
                    Title = m.Title,
                    Description = m.Description,
                    Priority = m.Priority,
                    Status = m.Status,
                    CreatedAt = m.CreatedAt
                })
                .FirstOrDefaultAsync();


            if (request == null)
                return NotFound();


            return Ok(request);
        }


        [HttpPost]
        public async Task<IActionResult> CreateRequest(MaintenanceRequestCreateDto dto)
        {
            var roomExists = await _context.Rooms
                .AnyAsync(r => r.Id == dto.RoomId);


            if (!roomExists)
                return BadRequest("Oda bulunamadı.");



            var request = new MaintenanceRequest
            {
                RoomId = dto.RoomId,
                EmployeeId = dto.EmployeeId,
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = dto.Status,
                CreatedAt = DateTime.Now
            };


            _context.MaintenanceRequests.Add(request);
            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Bakım talebi oluşturuldu.",
                id = request.Id
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.MaintenanceRequests
                .FindAsync(id);


            if (request == null)
                return NotFound();


            _context.MaintenanceRequests.Remove(request);
            await _context.SaveChangesAsync();


            return NoContent();
        }
    }
}