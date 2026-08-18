using HotelOS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class AdminController : Controller
    {
        private readonly HotelDbContext _context;

        public AdminController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalRooms = await _context.Rooms.CountAsync();

            var today = DateTime.UtcNow.Date;

            var activeReservations = await _context.Reservations
                .CountAsync(r => r.CheckOut >= today);

            var totalCustomers = await _context.Customers.CountAsync();

            var todayIncome = await _context.Payments
     .Where(p => p.PaymentDate >= DateTime.UtcNow.Date &&
                 p.PaymentDate < DateTime.UtcNow.Date.AddDays(1))
     .SumAsync(p => (decimal?)p.Amount) ?? 0;

            ViewBag.TotalRooms = totalRooms;
            ViewBag.ActiveReservations = activeReservations;
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.TodayIncome = todayIncome;

            return View();
        }
    }
}