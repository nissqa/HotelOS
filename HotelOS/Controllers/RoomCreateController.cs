using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class RoomCreateController : Controller
    {
        private readonly HotelDbContext _context;

        public RoomCreateController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Index()
        {
            ViewBag.RoomTypes = await _context.RoomTypes
                .OrderBy(x => x.TypeName)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(
            string roomNumber,
            int roomTypeId,
            int floor,
            string? description)
        {
            if (string.IsNullOrWhiteSpace(roomNumber))
            {
                TempData["ErrorMessage"] = "Oda numarası boş bırakılamaz.";
                return RedirectToAction("Index");
            }

            if (await _context.Rooms.AnyAsync(x => x.RoomNumber == roomNumber))
            {
                TempData["ErrorMessage"] = "Bu oda numarası zaten mevcut.";
                return RedirectToAction("Index");
            }

            var room = new Room
            {
                RoomNumber = roomNumber,
                RoomTypeId = roomTypeId,
                Floor = floor,
                Description = description,
                Status = Models.Enums.RoomStatus.Available,
                IsActive = true
            };

            _context.Rooms.Add(room);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Oda başarıyla eklendi.";

            return RedirectToAction("Index", "RoomsView");
        }
    }
}