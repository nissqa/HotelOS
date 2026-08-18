using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CleaningManagementController : Controller
    {
        private readonly HotelDbContext _context;

        public CleaningManagementController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.CleaningTasks
                .Include(t => t.Room)
                .Include(t => t.Employee)
                .OrderByDescending(t => t.TaskDate)
                .ToListAsync();

            return View(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> AssignTask(
            int roomId,
            int employeeId,
            DateTime taskDate,
            string? notes)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
            {
                TempData["Error"] = "Oda bulunamadı.";
                return RedirectToAction("Index");
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                TempData["Error"] = "Temizlik görevlisi bulunamadı.";
                return RedirectToAction("Index");
            }

            var task = new CleaningTask
            {
                RoomId = roomId,
                EmployeeId = employeeId,
                TaskDate = taskDate,
                Status = "Bekliyor",
                Notes = notes
            };

            _context.CleaningTasks.Add(task);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Oda {room.RoomNumber}, {employee.FirstName} {employee.LastName} adlı görevliye atandı.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.CleaningTasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                TempData["Error"] = "Temizlik görevi bulunamadı.";
                return RedirectToAction("Index");
            }

            _context.CleaningTasks.Remove(task);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Temizlik görevi silindi.";

            return RedirectToAction("Index");
        }
    }
}