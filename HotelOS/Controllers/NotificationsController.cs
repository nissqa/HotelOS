using HotelOS.Data;
using HotelOS.DTOs;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public NotificationsController(HotelDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
        {
            var notifications = await _context.Notifications
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();


            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDto>> GetNotification(int id)
        {
            var notification = await _context.Notifications
                .Where(n => n.Id == id)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .FirstOrDefaultAsync();



            if (notification == null)
                return NotFound();


            return Ok(notification);
        }



        [HttpPost]
        public async Task<IActionResult> CreateNotification(NotificationCreateDto dto)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == dto.UserId);


            if (!userExists)
                return BadRequest("Kullanıcı bulunamadı.");



            var notification = new Notification
            {
                UserId = dto.UserId,
                Title = dto.Title,
                Message = dto.Message,
                IsRead = false,
                CreatedAt = DateTime.Now
            };


            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();



            return Ok(new
            {
                message = "Bildirim oluşturuldu.",
                id = notification.Id
            });
        }



        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications
                .FindAsync(id);


            if (notification == null)
                return NotFound();


            notification.IsRead = true;

            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "Bildirim okundu olarak işaretlendi."
            });
        }





        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _context.Notifications
                .FindAsync(id);


            if (notification == null)
                return NotFound();


            _context.Notifications.Remove(notification);

            await _context.SaveChangesAsync();


            return NoContent();
        }
    }
}