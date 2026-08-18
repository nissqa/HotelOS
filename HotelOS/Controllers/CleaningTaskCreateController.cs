using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CleaningTaskCreateController : Controller
    {
        private readonly HotelDbContext _context;

        public CleaningTaskCreateController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cleaningEmployees = await _context.Employees
                .Where(e =>
                    e.Position == "Housekeeper" ||
                    e.Position == "Temizlik")
                .OrderBy(e => e.FirstName)
                .ToListAsync();

            ViewBag.CleaningEmployees = cleaningEmployees;

            var rooms = await _context.Rooms
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            ViewBag.Rooms = rooms;

            return View();
        }
    }
}