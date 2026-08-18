using HotelOS.Data;
using HotelOS.Helpers;
using HotelOS.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HotelOS.Controllers
{
    public class LoginController : Controller
    {
        private readonly HotelDbContext _context;

        public LoginController(HotelDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email ve şifre giriniz.";
                return View();
            }

            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                ViewBag.Error = "Kullanıcı bulunamadı.";
                return View();
            }

            if (!user.IsActive)
            {
                ViewBag.Error = "Bu kullanıcı hesabı aktif değil.";
                return View();
            }


            bool passwordCorrect;

            try
            {
                passwordCorrect = PasswordHelper.VerifyPassword(
                    password,
                    user.PasswordHash
                );
            }
            catch
            {
                ViewBag.Error =
                    "Kullanıcının şifre kaydı geçersiz. Şifre yeniden oluşturulmalıdır.";

                return View();
            }

            if (!passwordCorrect)
            {
                ViewBag.Error = "Şifre hatalı.";
                return View();
            }


            if (user.Role == null)
            {
                ViewBag.Error = "Kullanıcının rolü bulunamadı.";
                return View();
            }

            string roleName = user.Role.RoleName;


            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    user.Username ?? ""
                ),

                new Claim(
                    ClaimTypes.Email,
                    user.Email ?? ""
                ),

                new Claim(
                    ClaimTypes.Role,
                    roleName
                )
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );


            switch (roleName)
            {
                case "Admin":
                    return RedirectToAction("Index", "Admin");

                case "Reception":
                case "Receptionist":
                    return RedirectToAction("Index", "Reception");

                case "Cleaning":
                case "Housekeeper":
                    return RedirectToAction("Index", "CleaningPanel");

                case "Maintenance":
                    return RedirectToAction("Index", "Maintenance");

                case "BOIManager":
                    return RedirectToAction("Index", "BOI");

                case "Vale":
                    return RedirectToAction("Index", "ValePanel");

                default:
                    return RedirectToAction("Index", "Dashboard");
            }
        }



        [HttpGet]
        public IActionResult SetupReceptionPassword()
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Id == 10);

            if (user == null)
            {
                return Content(
                    "ID 10 olan Receptionist kullanıcısı bulunamadı."
                );
            }

            user.PasswordHash =
                PasswordHelper.HashPassword("123456");

            _context.SaveChanges();

            return Content(
                "Receptionist şifresi başarıyla ayarlandı. Şifre: 123456"
            );
        }

       
        [HttpGet]
        public async Task<IActionResult> SetupRealUsers()
        {
            

            var usersToCreate = new List<(string Username, string Email, string Password, string FirstName, string LastName, int RoleId)>
            {
            
                ("admin1", "gercek.admin1@oteliniz.com", "GecikSifre123!", "Ad1", "Soyad1", 5),
                ("admin2", "gercek.admin2@oteliniz.com", "GecikSifre123!", "Ad2", "Soyad2", 5),
                ("admin3", "gercek.admin3@oteliniz.com", "GecikSifre123!", "Ad3", "Soyad3", 5),

              
                ("temizlik1", "gercek.temizlik1@oteliniz.com", "GecikSifre123!", "Ad4", "Soyad4", 7),
                ("temizlik2", "gercek.temizlik2@oteliniz.com", "GecikSifre123!", "Ad5", "Soyad5", 7),
            };

            var results = new List<string>();

            foreach (var u in usersToCreate)
            {
                var exists = await _context.Users
                    .AnyAsync(x => x.Email == u.Email);

                if (exists)
                {
                    results.Add($"⏭️ Atlandı (zaten var): {u.Email}");
                    continue;
                }

                var newUser = new User
                {
                    Username = u.Username,
                    Email = u.Email,
                    PasswordHash = PasswordHelper.HashPassword(u.Password),
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    RoleId = u.RoleId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                results.Add($"✅ Eklendi: {u.Email} (RoleId: {u.RoleId})");
            }

            await _context.SaveChangesAsync();

            return Content(string.Join("\n", results));
        }




        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Index", "Login");
        }
    }
}