using HotelOS.Data;
using HotelOS.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class RoomOperationsController : Controller
    {
        private readonly HotelDbContext _context;


    public RoomOperationsController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms
                .Include(r => r.RoomType)
                .Where(r =>
                    r.Status == RoomStatus.Maintenance ||
                    r.Status == RoomStatus.Cleaning)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            return View(rooms);
        }
    }


}
