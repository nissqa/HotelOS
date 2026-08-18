using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class PoolController : Controller
    {
        private readonly HotelDbContext _context;

        public PoolController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pools = await _context.Pools
                .Include(p => p.AssignedEmployee)
                .OrderBy(p => p.Id)
                .ToListAsync();

            return View("~/Views/BOI/Pool.cshtml", pools);
        }
    }
}