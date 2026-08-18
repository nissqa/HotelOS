using HotelOS.Data;
using HotelOS.DTOs;
using HotelOS.Helpers;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HotelDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(HotelDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return Unauthorized("E-posta veya şifre hatalı.");
            }

           
            if (!PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return Unauthorized("E-posta veya şifre hatalı.");
            }

           
            var token = JwtHelper.GenerateToken(user, _configuration);

            return Ok(new
            {
                message = "Giriş başarılı.",
                token = token,
                username = user.Username,
                role = user.Role?.RoleName
            });
        }
        [HttpPost("register")]
       public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Bu e-posta zaten kayıtlı.");

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Bu kullanıcı adı zaten kullanılıyor.");

            var role = await _context.Roles.FindAsync(dto.RoleId);

            if (role == null)
                return BadRequest("Geçersiz rol.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                RoleId = dto.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Kullanıcı başarıyla oluşturuldu.");
        }

        [HttpPost("reset-admin-password")]
        public async Task<IActionResult> ResetAdminPassword()
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == 6);

            if (user == null)
                return NotFound("Admin bulunamadı.");

            user.PasswordHash = PasswordHelper.HashPassword("123456");

            await _context.SaveChangesAsync();

            return Ok("Admin şifresi 123456 olarak güncellendi.");
        }


    }
}