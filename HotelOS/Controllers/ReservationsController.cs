using HotelOS.Data;
using HotelOS.DTOs;
using HotelOS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public ReservationsController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Room)
                    .ThenInclude(r => r.RoomType)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Room)
                    .ThenInclude(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            return reservation;
        }

        [HttpPost]
        public async Task<ActionResult<Reservation>> CreateReservation(CreateReservationDto dto)
        {
            var customer = await _context.Customers.FindAsync(dto.CustomerId);

            if (customer == null)
                return BadRequest("Müşteri bulunamadı.");

            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == dto.RoomId);

            if (room == null)
                return BadRequest("Oda bulunamadı.");

            if (dto.CheckOut <= dto.CheckIn)
                return BadRequest("Çıkış tarihi giriş tarihinden sonra olmalıdır.");

            if (dto.AdultCount <= 0)
                return BadRequest("En az 1 yetişkin olmalıdır.");

            bool conflict = await _context.Reservations.AnyAsync(r =>
                r.RoomId == dto.RoomId &&
                dto.CheckIn < r.CheckOut &&
                dto.CheckOut > r.CheckIn);

            if (conflict)
                return BadRequest("Bu oda seçilen tarihler arasında doludur.");

            int nightCount = (dto.CheckOut - dto.CheckIn).Days;

            decimal totalPrice = nightCount * room.RoomType!.PricePerNight;

            var reservation = new Reservation
            {
                CustomerId = dto.CustomerId,
                RoomId = dto.RoomId,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                AdultCount = dto.AdultCount,
                ChildCount = dto.ChildCount,
                Status = dto.Status,
                TotalPrice = totalPrice,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, Reservation reservation)
        {
            if (id != reservation.Id)
                return BadRequest();

            _context.Entry(reservation).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
                return NotFound();

            _context.Reservations.Remove(reservation);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}