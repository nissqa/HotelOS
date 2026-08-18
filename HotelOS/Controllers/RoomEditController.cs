using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class RoomEditController : Controller
    {
        private readonly HotelDbContext _context;

        public RoomEditController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
                return NotFound();

            ViewBag.RoomTypes = await _context.RoomTypes
                .OrderBy(r => r.TypeName)
                .ToListAsync();

            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> Index(
            int id,
            string roomNumber,
            int roomTypeId,
            int floor,
            string? description)
        {
            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(roomNumber))
            {
                ViewBag.Error = "Oda numarası boş bırakılamaz.";
            }
            else if (floor < 0 || floor > 10)
            {
                ViewBag.Error = "Kat bilgisi 0 ile 10 arasında olmalıdır.";
            }
            else if (await _context.Rooms.AnyAsync(r =>
                r.RoomNumber == roomNumber && r.Id != id))
            {
                ViewBag.Error = "Bu oda numarası başka bir odada kullanılıyor.";
            }
            else if (!await _context.RoomTypes.AnyAsync(r => r.Id == roomTypeId))
            {
                ViewBag.Error = "Geçersiz oda tipi.";
            }
            else
            {
                room.RoomNumber = roomNumber;
                room.RoomTypeId = roomTypeId;
                room.Floor = floor;
                room.Description = description;

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "RoomsView");
            }

            ViewBag.RoomTypes = await _context.RoomTypes
                .OrderBy(r => r.TypeName)
                .ToListAsync();

            return View(room);
        }
    }
}