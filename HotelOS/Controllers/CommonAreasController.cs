using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class CommonAreasController : Controller
    {
        private readonly HotelDbContext _context;

        public CommonAreasController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            
            var commonAreas = await _context.CommonAreas
                .OrderBy(a => a.Name)
                .ToListAsync();

           
            var cleaningEmployees = await _context.Employees
                .Where(e =>
                    e.Position == "Cleaning" ||
                    e.Position == "Housekeeper" ||
                    e.Position == "Temizlik Personeli"
                )
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();

           
            var maintenanceEmployees = await _context.Employees
                .Where(e =>
                    e.Position == "Maintenance" ||
                    e.Position == "Teknik Personel" ||
                    e.Position == "Teknik Sorumlu"
                )
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();

            
            var model = new CommonAreasViewModel
            {
                Areas = commonAreas,
                CleaningEmployees = cleaningEmployees,
                MaintenanceEmployees = maintenanceEmployees
            };

            return View("~/Views/BOI/CommonAreas.cshtml", model);
        }
    }
}