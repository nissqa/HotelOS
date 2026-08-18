using HotelOS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ValeController : Controller
{
    private readonly HotelDbContext _context;

    public ValeController(HotelDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vehicles = await _context.Vehicles
            .Include(v => v.CreatedByUser)
            .ThenInclude(u => u.Role)
            .Where(v =>
                v.CreatedByUser != null &&
                v.CreatedByUser.Role != null &&
                v.CreatedByUser.Role.RoleName == "Vale"
            )
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

        return View("~/Views/BOI/Vale.cshtml", vehicles);
    }
}