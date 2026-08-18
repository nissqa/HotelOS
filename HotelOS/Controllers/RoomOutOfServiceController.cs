
using HotelOS.Data;
using HotelOS.Models;
using HotelOS.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    [Route("RoomOutOfService")]
    public class RoomOutOfServiceController : Controller
    {
        private readonly HotelDbContext _context;

        public RoomOutOfServiceController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
                return NotFound();

            return View(room);
        }

        [HttpPost("Index")]
        public async Task<IActionResult> Index(
            int roomId,
            string reason,
            string? description,
            DateTime? expectedEndDate)
        {
            var room = await _context.Rooms.FindAsync(roomId);

            if (room == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(reason))
            {
                ViewBag.Error = "Lütfen kullanım dışı bırakma sebebini seçin.";
                return View(room);
            }

            DateTime? expectedEndDateUtc = null;

            if (expectedEndDate.HasValue)
            {
                expectedEndDateUtc = expectedEndDate.Value.Kind == DateTimeKind.Utc
                    ? expectedEndDate.Value
                    : expectedEndDate.Value.ToUniversalTime();
            }

            var record = new RoomOutOfService
            {
                RoomId = roomId,
                Reason = reason,
                Description = description,
                StartDate = DateTime.UtcNow,
                ExpectedEndDate = expectedEndDateUtc,
                IsActive = true
            };

            _context.RoomOutOfServices.Add(record);

            room.IsActive = false;
            room.Status = RoomStatus.OutOfService;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "RoomsView");
        }

        [HttpPost("Activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.OutOfServiceRecords)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
                return NotFound();

            var activeRecord = room.OutOfServiceRecords
                .FirstOrDefault(x => x.IsActive);

            if (activeRecord != null)
            {
                activeRecord.IsActive = false;
                activeRecord.EndDate = DateTime.UtcNow;
            }

            room.IsActive = true;
            room.Status = RoomStatus.Available;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "RoomsView");
        }
    }
}

