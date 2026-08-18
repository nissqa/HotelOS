using HotelOS.Data;
using HotelOS.Models;
using HotelOS.Models.Enums;
using HotelOS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    [Authorize]

    public class CheckInController : Controller
    {
        private readonly HotelDbContext _context;

        public CheckInController(HotelDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);


            var reservations = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Room)
                .Where(r =>
                    r.CheckIn >= today &&
                    r.CheckIn < tomorrow &&
                    r.Status != "Cancelled")
                .OrderBy(r => r.CheckIn)
                .ToListAsync();


            var model = new CheckInViewModel
            {
                TodayReservations = reservations,

                PendingCheckIns = reservations
                    .Where(r => r.Status != "CheckedIn")
                    .ToList(),

                CompletedCheckIns = reservations
                    .Where(r => r.Status == "CheckedIn")
                    .ToList()
            };


            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Today(string? search)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var query = _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Room)
                .Where(r =>
                    r.CheckIn >= today &&
                    r.CheckIn < tomorrow &&
                    r.Status == "CheckedIn");

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(r =>
                    (r.Customer!.FirstName + " " + r.Customer!.LastName).Contains(term) ||
                    (r.Room != null && r.Room.RoomNumber.Contains(term)));
            }

            var reservations = await query
                .OrderBy(r => r.Room!.RoomNumber)
                .ToListAsync();

            ViewData["Search"] = search;

            return View(reservations);
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.Id == id);


            if (reservation == null)
            {
                TempData["Error"] = "Rezervasyon bulunamadı.";
                return RedirectToAction("Index");
            }


            if (reservation.Status == "CheckedIn")
            {
                TempData["Error"] = "Bu rezervasyon için zaten check-in yapılmış.";
                return RedirectToAction("Index");
            }


            if (reservation.Status == "CheckedOut")
            {
                TempData["Error"] = "Bu rezervasyon daha önce check-out yapılmış.";
                return RedirectToAction("Index");
            }


            if (reservation.Status == "Cancelled")
            {
                TempData["Error"] = "İptal edilmiş rezervasyona check-in yapılamaz.";
                return RedirectToAction("Index");
            }


            if (reservation.Room == null)
            {
                TempData["Error"] = "Rezervasyona ait oda bulunamadı.";
                return RedirectToAction("Index");
            }


            if (reservation.Room.Status != RoomStatus.Available)
            {
                TempData["Error"] = "Bu oda şu anda müsait değil.";
                return RedirectToAction("Index");
            }


            reservation.Status = "CheckedIn";
            reservation.Room.Status = RoomStatus.Occupied;


            await _context.SaveChangesAsync();


            TempData["Success"] =
                $"{reservation.Customer?.FirstName} {reservation.Customer?.LastName} için check-in başarıyla yapıldı.";


            return RedirectToAction("Index");
        }
    }
}