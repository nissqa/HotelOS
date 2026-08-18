using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class MaintenancePanelController : Controller
    {
        private readonly HotelDbContext _context;

        private static readonly string[] PriorityOrder =
        {
            "Düşük",
            "Normal",
            "Yüksek",
            "Acil",
            "Çok Acil"
        };

        public MaintenancePanelController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var activeRequests = await _context.MaintenanceRequests
                .Include(x => x.Room)
                .Include(x => x.Employee)
                .Where(x => x.Status != "Tamamlandı" && x.Status != "İptal")
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var completedRequests = await _context.MaintenanceRequests
                .Include(x => x.Room)
                .Include(x => x.Employee)
                .Where(x => x.Status == "Tamamlandı" || x.Status == "İptal")
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            ViewBag.MaintenanceEmployees = await _context.Employees
                .Where(e =>
                    e.Status == "Aktif" &&
                    (e.Position == "Bakım" || e.Position == "Maintenance"))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();

            ViewBag.CompletedRequests = completedRequests;

            return View("~/Views/BOI/Maintenance.cshtml", activeRequests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int id, int employeeId)
        {
            var request = await _context.MaintenanceRequests
                .Include(x => x.Room)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (request == null)
            {
                TempData["Error"] = "Bakım görevi bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e =>
                    e.Id == employeeId &&
                    e.Status == "Aktif" &&
                    (e.Position == "Bakım" || e.Position == "Maintenance"));

            if (employee == null)
            {
                TempData["Error"] = "Seçilen bakım personeli bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            request.EmployeeId = employee.Id;

            if (request.Status == null ||
                request.Status == "Bekliyor")
            {
                request.Status = "Devam Ediyor";
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Oda {request.Room?.RoomNumber} görevi " +
                $"{employee.FirstName} {employee.LastName} personeline atandı.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var request = await _context.MaintenanceRequests
                .FirstOrDefaultAsync(x => x.Id == id);

            if (request == null)
                return NotFound();

            request.Status = "Tamamlandı";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Bakım görevi tamamlandı olarak işaretlendi.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var request = await _context.MaintenanceRequests
                .FirstOrDefaultAsync(x => x.Id == id);

            if (request == null)
                return NotFound();

            request.Status = "İptal";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Bakım görevi iptal edildi.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Escalate(int id)
        {
            var request = await _context.MaintenanceRequests
                .FirstOrDefaultAsync(x => x.Id == id);

            if (request == null)
                return NotFound();

            var currentIndex = Array.IndexOf(
                PriorityOrder,
                request.Priority);

            if (currentIndex < 0)
                currentIndex = 1;

            var nextIndex = Math.Min(
                currentIndex + 1,
                PriorityOrder.Length - 1);

            request.Priority = PriorityOrder[nextIndex];

            await _context.SaveChangesAsync();

            TempData["Success"] = "Görevin önceliği yükseltildi.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _context.MaintenanceRequests
                .Include(x => x.Room)
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (request == null)
                return NotFound();

            return View("~/Views/MaintenanceRequest/Details.cshtml", request);
        }
    }
}