using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    
        [Authorize(Roles = "Cleaning")]
        
        public class CleaningPanelController : Controller
    {
        private readonly HotelDbContext _context;

        public CleaningPanelController(HotelDbContext context)
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

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var task = await _context.CleaningTasks
                .Include(t => t.Room)
                .Include(t => t.Employee)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound();

            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> StartCleaning(int id)
        {
            var task = await _context.CleaningTasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                TempData["Error"] = "Temizlik görevi bulunamadı.";
                return RedirectToAction("Index");
            }

            if (task.Status != "Bekliyor")
            {
                TempData["Error"] = "Bu temizlik görevi başlatılamaz.";
                return RedirectToAction("Index");
            }

            task.Status = "Temizlikte";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Temizlik görevi başlatıldı.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CompleteCleaning(int id)
        {
            var task = await _context.CleaningTasks
                .Include(t => t.Room)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                TempData["Error"] = "Temizlik görevi bulunamadı.";
                return RedirectToAction("Index");
            }

            if (task.Status != "Temizlikte")
            {
                TempData["Error"] = "Bu görev tamamlanamaz.";
                return RedirectToAction("Index");
            }

            task.Status = "Temizlendi";

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Oda {task.Room?.RoomNumber} temizliği tamamlandı.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SendMaintenance(int id, string? priority)
        {
            var task = await _context.CleaningTasks
                .Include(t => t.Room)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                TempData["Error"] = "Temizlik görevi bulunamadı.";
                return RedirectToAction("Index");
            }

            if (task.Status != "Bekliyor" &&
                task.Status != "Temizlikte")
            {
                TempData["Error"] =
                    "Bu görev bakım için gönderilemez.";

                return RedirectToAction("Index");
            }

            var allowedPriorities =
                new[] { "Normal", "Acil", "Çok Acil" };

            var selectedPriority =
                allowedPriorities.Contains(priority)
                    ? priority
                    : "Normal";

            var existingRequest =
                await _context.MaintenanceRequests
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
                    Description =
                        string.IsNullOrWhiteSpace(task.Notes)
                            ? $"Oda {task.Room?.RoomNumber} için bakım talebi oluşturuldu."
                            : task.Notes,
                    Priority = selectedPriority,
                    Status = "Bekliyor",
                    CreatedAt = DateTime.UtcNow
                };

                _context.MaintenanceRequests.Add(maintenanceRequest);
            }

            task.Status = "Bakım";

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Oda {task.Room?.RoomNumber} bakım ekibine gönderildi.";

            return RedirectToAction("Index");
        }
    }
}