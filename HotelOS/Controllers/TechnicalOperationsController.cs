using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class TechnicalOperationsController : Controller
    {
        private readonly HotelDbContext _context;

        public TechnicalOperationsController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var equipments = await _context.TechnicalEquipments
                .Where(e => e.IsActive)
                .OrderBy(e => e.Category)
                .ThenBy(e => e.Name)
                .ToListAsync();

            var maintenanceEmployees = await _context.Employees
    .Where(e =>
        (e.Position == "Maintenance" ||
         e.Position == "Teknik Personel")
        &&
        e.Status == "Active"
    )
    .OrderBy(e => e.FirstName)
    .ThenBy(e => e.LastName)
    .ToListAsync();

            ViewBag.MaintenanceEmployees = maintenanceEmployees;

            return View(
                "~/Views/BOI/TechnicalOperations.cshtml",
                equipments
            );
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inspect(int id)
        {
            var equipment = await _context.TechnicalEquipments
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipment == null)
                return NotFound();

            equipment.Status = "Kontrole Gönderildi";

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"{equipment.Name} kontrol için gönderildi.";

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendToMaintenance(
            int id,
            int employeeId,
            string? description)
        {
            var equipment = await _context.TechnicalEquipments
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipment == null)
                return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e =>
                    e.Id == employeeId &&
                    e.Status == "Aktif" &&
                    (
                        e.Position == "Maintenance" ||
                        e.Position == "Teknik Personel" ||
                        e.Position == "Teknik Sorumlu"
                    )
                );

            if (employee == null)
            {
                TempData["Error"] =
                    "Geçerli bir teknik personel seçmelisiniz.";

                return RedirectToAction(nameof(Index));
            }

            var request = new MaintenanceRequest
            {
                RoomId = 0,
                TechnicalEquipmentId = equipment.Id,
                EmployeeId = employee.Id,
                Title = $"Teknik Ekipman Bakımı - {equipment.Name}",
                Description = description,
                Priority = "Normal",
                Status = "Bekliyor",
                CreatedAt = DateTime.UtcNow
            };

            _context.MaintenanceRequests.Add(request);

            equipment.Status = "Bakım Gerekiyor";

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"{equipment.Name} bakıma gönderildi. " +
                $"Görevli: {employee.FirstName} {employee.LastName}";

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReportFault(
            int id,
            string? description)
        {
            var equipment = await _context.TechnicalEquipments
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipment == null)
                return NotFound();

            equipment.Status = "Arızalı";

            if (!string.IsNullOrWhiteSpace(description))
            {
                equipment.Description = description;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"{equipment.Name} için arıza kaydı oluşturuldu.";

            return RedirectToAction(nameof(Index));
        }
    }
}