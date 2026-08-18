using HotelOS.Data;
using HotelOS.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class RoomsViewController : Controller
    {
        private readonly HotelDbContext _context;

        public RoomsViewController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.Reservations)
                .ToListAsync();

            var now = DateTime.UtcNow;

            foreach (var room in rooms)
            {
                if (room.Status == RoomStatus.Maintenance ||
                    room.Status == RoomStatus.OutOfService)
                {
                    continue;
                }

                bool hasActiveReservation = room.Reservations.Any(r =>
                    r.CheckIn <= now &&
                    r.CheckOut > now
                );

                if (hasActiveReservation)
                {
                    room.Status = RoomStatus.Occupied;
                }
                else
                {
                    room.Status = RoomStatus.Available;
                }
            }

            return View(rooms);
        }

        [HttpGet]
        public async Task<IActionResult> Occupied(string? search)
        {
            var rooms = await _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.Reservations)
                    .ThenInclude(res => res.Customer)
                .ToListAsync();

            var now = DateTime.UtcNow;

            foreach (var room in rooms)
            {
                if (room.Status == RoomStatus.Maintenance ||
                    room.Status == RoomStatus.OutOfService)
                {
                    continue;
                }

                bool hasActiveReservation = room.Reservations.Any(r =>
                    r.CheckIn <= now &&
                    r.CheckOut > now
                );

                room.Status = hasActiveReservation ? RoomStatus.Occupied : RoomStatus.Available;
            }

            var occupiedRooms = rooms
                .Where(r => r.Status == RoomStatus.Occupied)
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                occupiedRooms = occupiedRooms.Where(r => r.RoomNumber.Contains(term));
            }

            ViewData["Search"] = search;

            return View(occupiedRooms.OrderBy(r => r.RoomNumber).ToList());
        }

       
        [HttpGet]
        public async Task<IActionResult> DetailJson(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.Reservations)
                    .ThenInclude(res => res.Customer)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var now = DateTime.UtcNow;

            var activeReservation = room.Reservations
                .Where(r => r.CheckIn <= now && r.CheckOut > now)
                .OrderByDescending(r => r.CheckIn)
                .FirstOrDefault();

            if (activeReservation == null)
            {
                return Json(new
                {
                    roomNumber = room.RoomNumber,
                    roomType = room.RoomType?.TypeName ?? "-",
                    hasReservation = false
                });
            }

            var kalanGun = Math.Max(0, (activeReservation.CheckOut.Date - now.Date).Days);

            return Json(new
            {
                roomNumber = room.RoomNumber,
                roomType = room.RoomType?.TypeName ?? "-",
                hasReservation = true,
                guestName = $"{activeReservation.Customer?.FirstName} {activeReservation.Customer?.LastName}",
                adultCount = activeReservation.AdultCount,
                childCount = activeReservation.ChildCount,
                checkIn = activeReservation.CheckIn.ToString("dd.MM.yyyy"),
                checkOut = activeReservation.CheckOut.ToString("dd.MM.yyyy"),
                kalanGun
            });
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .Include(r => r.Reservations)
                    .ThenInclude(res => res.Customer)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var now = DateTime.UtcNow;

            var activeReservation = room.Reservations
                .Where(r => r.CheckIn <= now && r.CheckOut > now)
                .OrderByDescending(r => r.CheckIn)
                .FirstOrDefault();

            ViewData["Room"] = room;

            return View(activeReservation);
        }
    }
}