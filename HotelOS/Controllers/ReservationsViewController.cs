using HotelOS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class ReservationsViewController : Controller
    {
        private readonly HotelDbContext _context;

        public ReservationsViewController(HotelDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var query = _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Room)
                    .ThenInclude(r => r.RoomType)
                .AsQueryable();

           

            var reservations = await query
                .OrderByDescending(r => r.CheckIn)
                .ToListAsync();

            

            return View(reservations);
        }
    }
}