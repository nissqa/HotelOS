using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class MaintenanceRequestCreateController : Controller
    {
        private readonly HotelDbContext _context;

        public MaintenanceRequestCreateController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Rooms = await _context.Rooms
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            ViewBag.Employees = await _context.Employees
                .Where(e => e.Status == "Aktif" &&
                            (e.Position == "Bakım" ||
                             e.Position == "Maintenance"))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(
            int roomId,
            int? employeeId,
            string? title,
            string? description,
            string? priority)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
            {
                TempData["Error"] = "Oda bulunamadı.";
                return RedirectToAction("Index");
            }

            if (employeeId.HasValue)
            {
                var employeeExists = await _context.Employees
                    .AnyAsync(e => e.Id == employeeId.Value &&
                                   e.Status == "Aktif" &&
                                   (e.Position == "Bakım" ||
                                    e.Position == "Maintenance"));

                if (!employeeExists)
                {
                    TempData["Error"] = "Seçilen bakım personeli bulunamadı.";
                    return RedirectToAction("Index");
                }
            }

            var request = new MaintenanceRequest
            {
                RoomId = roomId,
                EmployeeId = employeeId,
                Title = title,
                Description = description,
                Priority = priority,
                Status = "Bekliyor",
                CreatedAt = DateTime.UtcNow
            };

            _context.MaintenanceRequests.Add(request);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Oda {room.RoomNumber} için bakım talebi oluşturuldu.";

            return RedirectToAction("Index");
        }
    }
}