using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelOS.Controllers
{
    [Authorize(Roles = "Maintenance")]
    public class MaintenanceController : Controller
    {
        private readonly HotelDbContext _context;

        public MaintenanceController(HotelDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userIdText = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(userIdText, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email != null &&
                                          user.Email != null &&
                                          e.Email == user.Email);

            if (employee == null)
            {
                ViewBag.Error =
                    "Bu kullanıcıya bağlı bakım personeli kaydı bulunamadı.";

                ViewBag.Employee = null;
                ViewBag.CompletedTasks =
                    new List<HotelOS.Models.MaintenanceRequest>();

                return View(
                    "~/Views/Maintenance/Index.cshtml",
                    new List<HotelOS.Models.MaintenanceRequest>()
                );
            }


            var activeTasks = await _context.MaintenanceRequests
                .Include(x => x.Room)
                .Where(x =>
                    x.EmployeeId == employee.Id &&
                    x.Status != "Tamamlandı" &&
                    x.Status != "İptal")
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();


            var completedTasks = await _context.MaintenanceRequests
                .Include(x => x.Room)
                .Where(x =>
                    x.EmployeeId == employee.Id &&
                    (x.Status == "Tamamlandı" ||
                     x.Status == "İptal"))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            ViewBag.Employee = employee;
            ViewBag.CompletedRequests = completedTasks;

            return View(
                "~/Views/Maintenance/Index.cshtml",
                activeTasks
            );
        }



        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userIdText = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(userIdText, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email != null &&
                                          user.Email != null &&
                                          e.Email == user.Email);

            if (employee == null)
            {
                return NotFound();
            }

            
            var request = await _context.MaintenanceRequests
                .Include(x => x.Room)
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.EmployeeId == employee.Id);

            if (request == null)
            {
                return NotFound();
            }

            return View(
    "~/Views/MaintenanceRequest/Details.cshtml",
    request
);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var userIdText = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(userIdText, out int userId))
            {
                return RedirectToAction("Index", "Login");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email != null &&
                                          user.Email != null &&
                                          e.Email == user.Email);

            if (employee == null)
            {
                TempData["Error"] =
                    "Bu kullanıcıya bağlı bakım personeli bulunamadı.";

                return RedirectToAction(nameof(Index));
            }

            var request = await _context.MaintenanceRequests
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.EmployeeId == employee.Id);

            if (request == null)
            {
                TempData["Error"] =
                    "Bu görev size ait değil.";

                return RedirectToAction(nameof(Index));
            }

            request.Status = "Tamamlandı";

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Bakım görevi tamamlandı.";

            return RedirectToAction(nameof(Index));
        }
    }
}       