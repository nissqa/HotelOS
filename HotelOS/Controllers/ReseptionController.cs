using HotelOS.Data;
using HotelOS.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class ReceptionController : Controller
    {
        private readonly HotelDbContext _context;

        public ReceptionController(HotelDbContext context)
        {
            _context = context;
        }

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

            ViewBag.TodayReservations = reservations.Count;

            ViewBag.PendingCheckIns = reservations.Count(r => r.Status != "CheckedIn");

            ViewBag.CompletedCheckIns = reservations.Count(r => r.Status == "CheckedIn");

            ViewBag.AvailableRooms = await _context.Rooms
                .CountAsync(r => r.Status == RoomStatus.Available);

            ViewBag.TodayList = reservations;

            return View();
        }
    }
}