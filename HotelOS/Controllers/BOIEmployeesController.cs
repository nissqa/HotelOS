using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class BOIEmployeesController : Controller
    {
        private readonly HotelDbContext _context;

        public BOIEmployeesController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            string? position,
            string? status)
        {
           
            var employees = await _context.Employees
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                employees = employees
                    .Where(e =>
                        e.FirstName.ToLower().Contains(search) ||
                        e.LastName.ToLower().Contains(search) ||
                        (e.FirstName + " " + e.LastName)
                            .ToLower()
                            .Contains(search))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(position) &&
                position != "all")
            {
                employees = employees
                    .Where(e => e.Position == position)
                    .ToList();
            }


            if (!string.IsNullOrWhiteSpace(status) &&
                status != "all")
            {
                employees = employees
                    .Where(e => e.Status == status)
                    .ToList();
            }

            var positions = await _context.Employees
                .Where(e => !string.IsNullOrWhiteSpace(e.Position))
                .Select(e => e.Position!)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            ViewBag.Positions = positions;

            ViewBag.TotalEmployees =
                await _context.Employees.CountAsync();

            ViewBag.ActiveEmployees = await _context.Employees
    .CountAsync(e => e.Status == "Aktif");

            ViewBag.PassiveEmployees =
                await _context.Employees
                    .CountAsync(e => e.Status != "Aktif");

            ViewBag.Search = search;

            ViewBag.SelectedPosition = position;

            ViewBag.SelectedStatus = status;

            return View(
                "~/Views/BOI/Employees.cshtml",
                employees
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
    int id,
    string firstName,
    string lastName,
    string? phone,
    string? email,
    string? position,
    string status)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                TempData["Error"] = "Personel bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            employee.FirstName = firstName;
            employee.LastName = lastName;
            employee.Phone = phone;
            employee.Email = email;
            employee.Position = position;
            employee.Status = status;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"{employee.FirstName} {employee.LastName} bilgileri güncellendi.";

            return RedirectToAction(nameof(Index));
        }
    }
}