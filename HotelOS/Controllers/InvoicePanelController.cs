using HotelOS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class InvoicePanelController : Controller
    {
        private readonly HotelDbContext _context;


    public InvoicePanelController(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Reservation)
                    .ThenInclude(r => r.Customer)
                .Include(i => i.Reservation)
                    .ThenInclude(r => r.Room)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();

            return View(invoices);
        }

        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Reservation)
                    .ThenInclude(r => r.Customer)
                .Include(i => i.Reservation)
                    .ThenInclude(r => r.Room)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return PartialView("Details", invoice);
        }
    }

}
