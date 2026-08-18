using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class BOIController : Controller
    {
        private readonly HotelDbContext _context;

        public BOIController(HotelDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Bekleyen temizlik görevleri
            ViewBag.PendingCleaning = await _context.CleaningTasks
                .CountAsync(x =>
                    x.Status == "Bekliyor");

            // Devam eden temizlikler
            ViewBag.ActiveCleaning = await _context.CleaningTasks
                .CountAsync(x =>
                    x.Status == "Temizlikte");

            // Açık bakım talepleri
            ViewBag.PendingMaintenance = await _context.MaintenanceRequests
                .CountAsync(x =>
                    x.Status != "Tamamlandı" &&
                    x.Status != "İptal");

            ViewBag.ActiveEmployees = await _context.Employees
    .CountAsync();

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Cleaning()
        {
            var tasks = await _context.CleaningTasks
                .Include(x => x.Room)
                .Include(x => x.Employee)
                .Where(x =>
                    x.Status == "Bekliyor" ||
                    x.Status == "Temizlikte")
                .OrderBy(x => x.TaskDate)
                .ToListAsync();

            var employees = await _context.Employees
    .OrderBy(x => x.FirstName)
    .ThenBy(x => x.LastName)
    .ToListAsync();

            ViewBag.CleaningEmployees = employees;

            return View(tasks);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignCleaning(
            int taskId,
            int employeeId)
        {
            var task = await _context.CleaningTasks
                .Include(x => x.Room)
                .FirstOrDefaultAsync(x => x.Id == taskId);

            if (task == null)
            {
                TempData["Error"] =
                    "Temizlik görevi bulunamadı.";

                return RedirectToAction(nameof(Cleaning));
            }

            var employee = await _context.Employees
     .FirstOrDefaultAsync(x =>
         x.Id == employeeId);

            if (employee == null)
            {
                TempData["Error"] =
                    "Seçilen personel bulunamadı veya aktif değil.";

                return RedirectToAction(nameof(Cleaning));
            }

            task.EmployeeId = employee.Id;

            if (string.IsNullOrWhiteSpace(task.Status))
            {
                task.Status = "Bekliyor";
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Oda {task.Room?.RoomNumber} " +
                $"{employee.FirstName} {employee.LastName} " +
                "personeline atandı.";

            return RedirectToAction(nameof(Cleaning));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCleaningTask(
            int roomId,
            string? notes)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(x => x.Id == roomId);

            if (room == null)
            {
                TempData["Error"] =
                    "Oda bulunamadı.";

                return RedirectToAction(nameof(Cleaning));
            }

            var existingTask = await _context.CleaningTasks
                .AnyAsync(x =>
                    x.RoomId == roomId &&
                    (x.Status == "Bekliyor" ||
                     x.Status == "Temizlikte"));

            if (existingTask)
            {
                TempData["Error"] =
                    $"Oda {room.RoomNumber} için zaten aktif temizlik görevi var.";

                return RedirectToAction(nameof(Cleaning));
            }

            var task = new CleaningTask
            {
                RoomId = roomId,
                EmployeeId = 0,
                TaskDate = DateTime.UtcNow,
                Status = "Bekliyor",
                Notes = notes
            };

            _context.CleaningTasks.Add(task);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Oda {room.RoomNumber} için temizlik görevi oluşturuldu.";

            return RedirectToAction(nameof(Cleaning));
        }
    }
}