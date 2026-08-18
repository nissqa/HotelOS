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
    [Authorize(Roles = "Admin")]
    public class RoomTypesController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public RoomTypesController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomType>>> GetRoomTypes()
        {
            return await _context.RoomTypes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomType>> GetRoomType(int id)
        {
            var roomType = await _context.RoomTypes.FindAsync(id);

            if (roomType == null)
                return NotFound();

            return roomType;
        }

        [HttpPost]
        public async Task<ActionResult<RoomType>> CreateRoomType(CreateRoomTypeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TypeName))
                return BadRequest("Oda tipi boş bırakılamaz.");

            if (dto.Capacity <= 0)
                return BadRequest("Kapasite 0'dan büyük olmalıdır.");

            if (dto.PricePerNight <= 0)
                return BadRequest("Fiyat 0'dan büyük olmalıdır.");

            if (await _context.RoomTypes.AnyAsync(x => x.TypeName == dto.TypeName))
                return BadRequest("Bu oda tipi zaten mevcut.");

            var roomType = new RoomType
            {
                TypeName = dto.TypeName,
                Capacity = dto.Capacity,
                PricePerNight = dto.PricePerNight,
                Description = dto.Description
            };

            _context.RoomTypes.Add(roomType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoomType), new { id = roomType.Id }, roomType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoomType(int id, RoomType roomType)
        {
            if (id != roomType.Id)
                return BadRequest();

            _context.Entry(roomType).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomType(int id)
        {
            var roomType = await _context.RoomTypes.FindAsync(id);

            if (roomType == null)
                return NotFound();

            _context.RoomTypes.Remove(roomType);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}