using HotelOS.Data;
using HotelOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelOS.Controllers
{
    public class CustomerPanelController : Controller
    {
        private readonly HotelDbContext _context;

        public CustomerPanelController(HotelDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers
                .Include(c => c.Reservations)
                .ThenInclude(r => r.Room)
                .ToListAsync();

            var now = DateTime.Now;

            ViewBag.TotalCustomers = customers.Count;

            ViewBag.ActiveCustomers = customers.Count(c =>
                c.Reservations.Any(r =>
                    r.CheckIn <= now &&
                    r.CheckOut > now
                )
            );

            ViewBag.PastCustomers = customers.Count(c =>
                c.Reservations.Any(r =>
                    r.CheckOut <= now
                )
            );

            return View(customers);
        }



[HttpPost]
public async Task<IActionResult> Create(Customer customer)
        {
     
            customer.CreatedAt = DateTime.UtcNow;

           
            if (customer.BirthDate.HasValue)
            {
                customer.BirthDate = DateTime.SpecifyKind(
                    customer.BirthDate.Value,
                    DateTimeKind.Utc
                );
            }

            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Müşteri başarıyla kaydedildi.";

            return RedirectToAction("Index", "CustomerPanel");
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Reservations)
                .ThenInclude(r => r.Room)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
    }
}