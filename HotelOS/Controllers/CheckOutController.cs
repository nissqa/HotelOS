using HotelOS.Data;
using HotelOS.Models;
using HotelOS.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    [Authorize(Roles = "Admin,Reception,Receptionist")]
    public class CheckOutController : Controller
    {
        private readonly HotelDbContext _context;

        public CheckOutController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Room)
                .Where(r => r.Status == "CheckedIn")
                .OrderBy(r => r.CheckOut)
                .ToListAsync();

            return View(reservations);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                TempData["Error"] = "Rezervasyon bulunamadı.";
                return RedirectToAction("Index");
            }

            if (reservation.Status != "CheckedIn")
            {
                TempData["Error"] =
                    "Bu rezervasyon için check-out yapılamaz.";

                return RedirectToAction("Index");
            }

            if (reservation.Room == null)
            {
                TempData["Error"] =
                    "Rezervasyona ait oda bulunamadı.";

                return RedirectToAction("Index");
            }

            var roomId = reservation.RoomId;

            var cleaningEmployee = await _context.Employees
                .Where(e =>
                    e.Status == "Aktif" &&
                    e.Position != null &&
                    (
                        e.Position.ToLower().Contains("temizlik") ||
                        e.Position.ToLower().Contains("housekeeper")
                    ))
                .OrderBy(e => e.Id)
                .FirstOrDefaultAsync();

            if (cleaningEmployee == null)
            {
                TempData["Error"] =
                    "Check-out yapılamadı. Aktif temizlik personeli bulunamadı.";

                return RedirectToAction("Index");
            }


            reservation.Status = "CheckedOut";

            reservation.Room.Status = RoomStatus.Available;


            var existingCleaningTask = await _context.CleaningTasks
                .AnyAsync(t =>
                    t.RoomId == roomId &&
                    t.Status != "Temizlendi" &&
                    t.Status != "İptal");

            if (!existingCleaningTask)
            {
                var cleaningTask = new CleaningTask
                {
                    RoomId = roomId,
                    EmployeeId = cleaningEmployee.Id,
                    TaskDate = DateTime.UtcNow,
                    Status = "Bekliyor",
                    Notes = "Check-out sonrası oda temizliği"
                };

                _context.CleaningTasks.Add(cleaningTask);
            }



            var existingInvoice = await _context.Invoices
                .FirstOrDefaultAsync(i =>
                    i.ReservationId == reservation.Id);

            if (existingInvoice == null)
            {
                var invoice = new Invoice
                {
                    ReservationId = reservation.Id,

                    TotalAmount = reservation.TotalPrice,

                   
                    Tax = 2000m,

                   
                    IssueDate = DateTime.UtcNow,

                   
                    InvoiceNo =
                        "INV-" +
                        Guid.NewGuid()
                            .ToString("N")[..8]
                            .ToUpper()
                };

                _context.Invoices.Add(invoice);
            }


         

            await _context.SaveChangesAsync();


            TempData["Success"] =
                $"{reservation.Customer?.FirstName} " +
                $"{reservation.Customer?.LastName} " +
                " için check-out başarıyla yapıldı. " +
                "Fatura oluşturuldu ve oda temizlik personeline gönderildi.";

            return RedirectToAction("Index");
        }
    }
}